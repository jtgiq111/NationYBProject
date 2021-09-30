using Srvtools;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybdzdrjj : InfoForm
    {
        int _YBScrollValue = 0;
        int _HisScrollValue = 0;

        public Frm_ybdzdrjj()
        {
            InitializeComponent();
            this.dgvHisProject.AutoGenerateColumns = false;
            this.dgvYBProject.AutoGenerateColumns = false;
            this.dgvHisYBDZ.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                BindGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        void BindGrids()
        {
            string sql = "";
            string item = txtHisProjectName.Text.Trim();

            if (rbtnDrug.Checked)
            {
                sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                from yp01h a 
                left join bz05h b on a.y1sflb = b.b5sfno 
                where a.y1yplx in ('C', 'D', 'K', 'X', 'Z','W') 
                and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                if (rbtnHIsPYM.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1ypnm like '%{0}%'", item);
                    }
                }
            }
            else if (rbtnTherapy.Checked)
            {
                sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                from bz05d a 
                left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                if (rbtnHIsPYM.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.b5pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.b5name like '%{0}%'", item);
                    }
                }
            }
            else if (this.rbtnGHZC.Checked)
            {
                sql = @"select (a.bzcodn + a.bzkeyx) dm, a.bzname dmmc, len(a.bzname) lendmmc, null gg, a.bzname sflbmc, a.bzpymx pym
                from bztbd a 
                where a.bzcodn in ('A6', 'A7') 
                and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.bzcodn + a.bzkeyx))";

                if (rbtnHIsPYM.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.bzpymx like '%{0}%'", item);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.bzname like '%{0}%'", item);
                    }
                }
            }
            else if (this.rbtnYLK.Checked)
            {
                sql = @"select (a.pakind + a.pasequ) dm, a.paitnm dmmc, len(a.paitnm) lendmmc, null gg, a.paitnm sflbmc, null pym
                from patbh a 
                where a.pakind = 'MZ' 
                and a.pasequ = '0004' 
                and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.pakind + a.pasequ))";

                if (rbtnHIsPYM.Checked)
                {
                    //if (!string.IsNullOrWhiteSpace(item))
                    //{
                    //    sql += string.Format(" and a.bzpymx like '%{0}%'", item);
                    //}
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.paitnm like '%{0}%'", item);
                    }
                }
            }
            else if (rbtnCL.Checked)
            {
                sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                from yp01h a 
                left join bz05h b on a.y1sflb = b.b5sfno 
                where a.y1yplx in ('W') 
                and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                if (rbtnHIsPYM.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1ypnm like '%{0}%'", item);
                    }
                }
            }

            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            DataView dv = dt.DefaultView;
            dv.Sort = "dmmc, lendmmc";
            dgvHisProject.DataSource = dv;

            if (dgvHisProject.Rows.Count > _HisScrollValue)
            {
                dgvHisProject.FirstDisplayedScrollingRowIndex = _HisScrollValue;
            }

            BindDZGrids();
        }

        void BindDZGrids()
        {
            string sfzl;

            if (rbtnDrug.Checked)
            {
                sfzl = "'1'";
            }
            //else if (rbtnTherapy.Checked || rbtnGHZC.Checked || rbtnCL.Checked)
            //{
            //    sfzl = "2";
            //}
            else
            {
                sfzl = "'2', '3'";
            }

            string scbz;

            if (rbtnAlreadyUpload.Checked)
            {
                scbz = "1";
                btnscdz.Visible = false;
            }
            else
            {
                scbz = "0";
            }

            string sql = string.Format("select Hisxmbh, Hisxmmc, ybxmbh, ybxmmc, sfxmzldm from YBhisDZdr a where a.sfxmzldm in ({0}) and scbz = {1}", sfzl, scbz);
            string hisPYMxmmc = txtYYProjectName.Text.Trim();

            if (!string.IsNullOrWhiteSpace(hisPYMxmmc))
            {
                sql += string.Format(" and (a.Hispym like '%{0}%' or a.Hisxmmc like '%{0}%')", hisPYMxmmc);
            }

            sql += " order by id desc";
            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            dgvHisYBDZ.DataSource = dt;

            if (dt != null && dt.Rows.Count > 0)
            {
                if (rbtnNOUpload.Checked)
                {
                    btnscdz.Visible = true;
                }

                btnQXDZ.Visible = true;
            }
            else
            {
                btnscdz.Visible = false;
                btnQXDZ.Visible = false;
            }
        }

        private void btnQXDZ_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvHisYBDZ.CurrentRow != null && this.dgvHisYBDZ.CurrentRow.Index != -1 && dgvHisYBDZ.SelectedRows.Count > 0 && dgvHisYBDZ.SelectedRows[0].Cells[0].Value != null)
                {
                    string hisProjectCode = dgvHisYBDZ.CurrentRow.Cells[0].Value.ToString();
                    string hisProjectName = dgvHisYBDZ.CurrentRow.Cells[1].Value.ToString();
                    string YBProjectCode = dgvHisYBDZ.CurrentRow.Cells[2].Value.ToString();
                    string YBProjectName = dgvHisYBDZ.CurrentRow.Cells[3].Value.ToString();
                    object[] obj = { hisProjectCode, YBProjectCode };
                    obj = CliUtils.CallMethod("sybdj", "DeleteHisYBDZProject", obj);

                    if (obj[1].ToString() == "1")
                    {
                        MessageBox.Show("已取消对照", "提示");
                        BindGrids();
                    }
                    else
                    {
                        MessageBox.Show("取消失败", "提示");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请选择一条已对照记录", "提示");
                    return;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        /// <summary>
        /// 药品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnDrug_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                string item = txtHisProjectName.Text.Trim();

                if (rbtnHIsPYM.Checked)
                {
                    sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                    from yp01h a 
                    left join bz05h b on a.y1sflb = b.b5sfno 
                    where a.y1yplx in ('C', 'D', 'K', 'X', 'Z') 
                    and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                    from yp01h a 
                    left join bz05h b on a.y1sflb = b.b5sfno 
                    where a.y1yplx in ('C', 'D', 'K', 'X', 'Z') 
                    and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1ypnm like '%{0}%'", item);
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void rbtnTherapy_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                string item = txtHisProjectName.Text.Trim();

                if (rbtnHIsPYM.Checked)
                {
                    sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                    from bz05d a 
                    left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                    where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.b5pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                    from bz05d a left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                    where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.b5name like '%{0}%'", item);
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void btnDZ_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvHisProject.CurrentRow != null && dgvHisProject.CurrentRow.Index != -1)
                {

                }
                else
                {
                    MessageBox.Show("请选择一条医院项目记录", "提示");
                    return;
                }

                string hisProjectCode = dgvHisProject.CurrentRow.Cells[0].Value.ToString();
                string hisProjectName = dgvHisProject.CurrentRow.Cells[1].Value.ToString();
                string hisPYM = dgvHisProject.CurrentRow.Cells[4].Value.ToString();

                if (dgvYBProject.CurrentRow != null && this.dgvYBProject.CurrentRow.Index != -1)
                {

                }
                else
                {
                    MessageBox.Show("请选择一条医保项目记录", "提示");
                    return;
                }

                string YBProjectCode = dgvYBProject.CurrentRow.Cells[0].Value.ToString();
                string YBProjectName = dgvYBProject.CurrentRow.Cells[1].Value.ToString();
                object[] obj = new object[] { hisProjectCode, hisProjectName, YBProjectCode, YBProjectName, hisPYM, CliUtils.fLoginUser, CliUtils.fUserName };
                obj = CliUtils.CallMethod("sybdj", "InsertHisYBDZProject", obj);

                if (obj[1].ToString() == "1")
                {
                    MessageBox.Show(obj[2].ToString(), "提示");
                    BindGrids();
                }
                else
                {
                    MessageBox.Show("数据库操作失败" + obj[2].ToString(), "提示");
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void dgvHisProject_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    _HisScrollValue = e.NewValue;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void dgvYBProject_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    _YBScrollValue = e.NewValue;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void rbtnGHZC_Click(object sender, EventArgs e)
        {
            try
            {
                string item = txtHisProjectName.Text.Trim();
                string sql = @"select (a.bzcodn + a.bzkeyx) dm, a.bzname dmmc, len(a.bzname) lendmmc, null gg, a.bzname sflbmc, a.bzpymx pym
                from bztbd a where a.bzcodn in ('A6', 'A7') and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.bzcodn + a.bzkeyx))";

                if (rbtnHIsPYM.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.bzpymx like '%{0}%'", item);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.bzname like '%{0}%'", item);
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void rbtnYLK_Click(object sender, EventArgs e)
        {
            try
            {
                string item = txtHisProjectName.Text.Trim();
                string sql = @"select (a.pakind + a.pasequ) dm, a.paitnm dmmc, len(a.paitnm) lendmmc, null gg, a.paitnm sflbmc, null pym
                from patbh a 
                where a.pakind = 'MZ' 
                and a.pasequ = '0004' 
                and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.pakind + a.pasequ))";

                if (rbtnHIsPYM.Checked)
                {
                    //if (!string.IsNullOrWhiteSpace(item))
                    //{
                    //    sql += string.Format(" and a.bzpymx like '%{0}%'", item);
                    //}
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.paitnm like '%{0}%'", item);
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void rbtnJYJCZLZS_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                string item = txtHisProjectName.Text.Trim();

                if (rbtnHIsPYM.Checked)
                {
                    sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                    from bz05d a 
                    left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                    where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.b5pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                    from bz05d a 
                    left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                    where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.b5name like '%{0}%'", item);
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void rbtnYBPYM_Click(object sender, EventArgs e)
        {
            rbtnHIsPYM.Checked = true;

            if (!string.IsNullOrWhiteSpace(txtYBProjectName.Text))
            {
                txtYBProjectName_KeyUp(null, null);
            }

            if (!string.IsNullOrWhiteSpace(txtHisProjectName.Text))
            {
                txtHisProjectName_KeyUp(null, null);
            }
        }

        private void rbtnYBName_Click(object sender, EventArgs e)
        {
            rbtnHisItemName.Checked = true;

            if (!string.IsNullOrWhiteSpace(txtYBProjectName.Text))
            {
                txtYBProjectName_KeyUp(null, null);
            }

            if (!string.IsNullOrWhiteSpace(txtHisProjectName.Text))
            {
                txtHisProjectName_KeyUp(null, null);
            }
        }

        private void btnkssc_Click(object sender, EventArgs e)
        {
            object[] obj = yb_interface.ybs_interface("3500",new object[]{""});

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("科室信息上传成功", "提示");
            }
            else
            {
                MessageBox.Show("科室信息上传失败", "提示");
            }
        }

        private void btnyssc_Click(object sender, EventArgs e)
        {
            object[] obj = yb_interface.ybs_interface("3400",new object[]{});

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("医生信息上传成功", "提示");
            }
            else
            {
                MessageBox.Show("医生信息上传失败", "提示");
            }
        }

        private void btnscdz_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder dzmx = new StringBuilder();
                StringBuilder xmbh = new StringBuilder();

                foreach (DataGridViewRow dgvr in this.dgvHisYBDZ.Rows)
                {
                    string sfxmzldm = dgvr.Cells["sfxmzldm"].Value.ToString();
                    string ybxmbh = dgvr.Cells["ybxmbh"].Value.ToString();
                    string hisxmbh = dgvr.Cells["hisxmbh"].Value.ToString();
                    xmbh.Append("'" + hisxmbh + "',");
                    string hisxmmc = dgvr.Cells["hisxmmc"].Value.ToString();
                    DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                    dzmx.Append(sfxmzldm + "|" + ybxmbh + "|" + hisxmbh + "|" + hisxmmc + "|||||||" + dqsj.ToString("yyyyMMddHHmmss") + "|||||$");
                }

                if (dzmx.Length == 0)
                {
                    MessageBox.Show("无未上传对照记录", "提示");
                    return;
                }

                object[] obj = { dzmx.ToString().TrimEnd('$') };
                obj = yb_interface.ybs_interface("5202",obj);

                if (obj[1].ToString() == "1")
                {
                    MessageBox.Show("对照上传成功", "提示");
                    string xmbhs = xmbh.ToString().TrimEnd(',');
                    string sql = string.Format("update ybhisdzdr set scbz = 1 where hisxmbh in ({0})", xmbhs);
                    obj = new object[] { sql };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    MessageBox.Show("修改状态成功", "提示");
                    BindDZGrids();
                }
                else
                {
                    MessageBox.Show("对照上传失败");
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void rbtnHIsPYM_Click(object sender, EventArgs e)
        {
            rbtnYBPYM.Checked = true;

            if (!string.IsNullOrWhiteSpace(txtHisProjectName.Text))
            {
                txtHisProjectName_KeyUp(null, null);
            }

            if (!string.IsNullOrWhiteSpace(txtYBProjectName.Text))
            {
                txtYBProjectName_KeyUp(null, null);
            }
        }

        private void rbtnHisItemName_Click(object sender, EventArgs e)
        {
            rbtnYBName.Checked = true;

            if (!string.IsNullOrWhiteSpace(txtHisProjectName.Text))
            {
                txtHisProjectName_KeyUp(null, null);
            }

            if (!string.IsNullOrWhiteSpace(txtYBProjectName.Text))
            {
                txtYBProjectName_KeyUp(null, null);
            }
        }

        /// <summary>
        /// 材料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnCL_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                string item = txtHisProjectName.Text.Trim();

                if (rbtnHIsPYM.Checked)
                {
                    sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                    from yp01h a 
                    left join bz05h b on a.y1sflb = b.b5sfno 
                    where a.y1yplx in ('W') 
                    and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1pymx like '%{0}%'", item);
                    }
                }
                else
                {
                    sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                    from yp01h a 
                    left join bz05h b on a.y1sflb = b.b5sfno 
                    where a.y1yplx in ('W') 
                    and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        sql += string.Format(" and a.y1ypnm like '%{0}%'", item);
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void rbtnNOUpload_Click(object sender, EventArgs e)
        {
            BindDZGrids();
        }

        private void rbtnAlreadyUpload_Click(object sender, EventArgs e)
        {
            BindDZGrids();
        }

        private void txtHisProjectName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string sql = "";
                string item = txtHisProjectName.Text.Trim();

                if (rbtnHIsPYM.Checked)
                {
                    if (rbtnDrug.Checked)
                    {
                        sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                            from yp01h a 
                            left join bz05h b on a.y1sflb = b.b5sfno 
                            where a.y1yplx in ('C', 'D', 'K', 'X', 'Z') 
                            and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.y1pymx like '%{0}%'", item);
                        }
                    }
                    else if (rbtnTherapy.Checked)
                    {
                        sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                            from bz05d a 
                            left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                            where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.b5pymx like '%{0}%'", item);
                        }
                    }
                    else if (rbtnGHZC.Checked)
                    {
                        sql = @"select (a.bzcodn + a.bzkeyx) dm, a.bzname dmmc, len(a.bzname) lendmmc, null gg, a.bzname sflbmc, a.bzpymx pym
                            from bztbd a 
                            where a.bzcodn in ('A6', 'A7') 
                            and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.bzcodn + a.bzkeyx))";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.bzpymx like '%{0}%'", item);
                        }
                    }
                    else if (rbtnYLK.Checked)
                    {
                        sql = @"select (a.pakind + a.pasequ) dm, a.paitnm dmmc, len(a.paitnm) lendmmc, null gg, a.paitnm sflbmc, null pym
                            from patbh a 
                            where a.pakind = 'MZ' 
                            and a.pasequ = '0004' 
                            and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.pakind + a.pasequ))";
                    }
                    else if (rbtnCL.Checked)
                    {
                        sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                            from yp01h a 
                            left join bz05h b on a.y1sflb = b.b5sfno 
                            where a.y1yplx in ('W') 
                            and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.y1pymx like '%{0}%'", item);
                        }
                    }
                }
                else
                {
                    if (rbtnDrug.Checked)
                    {
                        sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                            from yp01h a 
                            left join bz05h b on a.y1sflb = b.b5sfno 
                            where a.y1yplx in ('C', 'D', 'K', 'X', 'Z') 
                            and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.y1ypnm like '%{0}%'", item);
                        }
                    }
                    else if (rbtnTherapy.Checked)
                    {
                        sql = @"select a.b5item dm, a.b5name dmmc, len(a.b5name) lendmmc, a.b5unit gg, b.b5sfnm sflbmc, a.b5pymx pym
                            from bz05d a 
                            left join bz05h b on a.b5comp = b.b5comp and a.b5sfno = b.b5sfno 
                            where not exists (select 1 from YBhisDZdr c where c.hisxmbh = a.b5item)";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.b5name like '%{0}%'", item);
                        }
                    }
                    else if (rbtnGHZC.Checked)
                    {
                        sql = @"select (a.bzcodn + a.bzkeyx) dm, a.bzname dmmc, len(a.bzname) lendmmc, null gg, a.bzname sflbmc, a.bzpymx pym
                            from bztbd a 
                            where a.bzcodn in ('A6', 'A7') 
                            and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.bzcodn + a.bzkeyx))";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.bzname like '%{0}%'", item);
                        }
                    }
                    else if (rbtnYLK.Checked)
                    {
                        sql = @"select (a.pakind + a.pasequ) dm, a.paitnm dmmc, len(a.paitnm) lendmmc, null gg, a.paitnm sflbmc, null pym
                            from patbh a 
                            where a.pakind = 'MZ' 
                            and a.pasequ = '0004' 
                            and not exists (select 1 from YBhisDZdr c where c.hisxmbh = (a.pakind + a.pasequ))";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.paitnm like '%{0}%'", item);
                        }
                    }
                    else if (rbtnCL.Checked)
                    {
                        sql = @"select a.y1ypno dm, a.y1ypnm dmmc, len(a.y1ypnm) lendmmc, a.y1ggxx gg, b.b5sfnm sflbmc, a.y1pymx pym
                            from yp01h a 
                            left join bz05h b on a.y1sflb = b.b5sfno 
                            where a.y1yplx in ('W') 
                            and a.y1ypno not in (select Hisxmbh from YBhisDZdr)";

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            sql += string.Format(" and a.y1ypnm like '%{0}%'", item);
                        }
                    }
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvHisProject.DataSource = dv;
                txtYBProjectName.Text = item;
                txtYBProjectName_KeyUp(sender, e);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void txtYBProjectName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string projectName = this.txtYBProjectName.Text.Trim();
                string sql = @"select a.dm, a.dmmc, len(a.dmmc) lendmmc, a.sfxmdj, a.sfxmdjdm type, a.jx, a.gg, a.sccj from ybMRdr a where 1 = 1";

                if (rbtnYBPYM.Checked)
                {
                    sql += string.Format(" and a.pym like '%{0}%'", projectName);
                }
                else
                {
                    sql += string.Format(" and a.dmmc like '%{0}%'", projectName);
                }

                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                DataView dv = dt.DefaultView;
                dv.Sort = "dmmc, lendmmc";
                dgvYBProject.DataSource = dv;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void txtYYProjectName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.txtYYProjectName.Text))
                {
                    return;
                }

                BindDZGrids();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_xz_Click(object sender, EventArgs e)
        {
            object[] obj = { cmbsjlb.Text.Split('|')[0] };
            obj = yb_interface.ybs_interface("5108",obj);
            MessageBox.Show(obj[2].ToString());
        }
    }
}
