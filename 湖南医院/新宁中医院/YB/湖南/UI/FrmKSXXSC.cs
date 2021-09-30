using AutoListView;
using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace yb_interfaces
{
    public partial class FrmKSXXSC : InfoForm
    {
        private DataSet _catyTable;
        public FrmKSXXSC()
        {
            InitializeComponent();
        }

        private void FrmKSXXSC_Load(object sender, EventArgs e)
        {
            alvCaty.SetColumnParam.AddQueryColumn("bzkeyx");
            alvCaty.SetColumnParam.AddQueryColumn("bzname");
            alvCaty.SetColumnParam.AddQueryColumn("bzwbmx");
            alvCaty.SetColumnParam.AddQueryColumn("bzpymx");

            alvCaty.SetColumnParam.AddViewColumn("bzkeyx", "编码", 100);
            alvCaty.SetColumnParam.AddViewColumn("bzname", "名称", 120);

            alvCaty.SetColumnParam.SetTextColumn("bzname");
            alvCaty.SetColumnParam.SetValueColumn("bzkeyx");

            string strSql = "select bzkeyx,bzname,bzwbmx,bzpymx from bztbd where bzcodn = 'dept' and bzusex = 1";
            DataSet dsCS = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            _catyTable = dsCS.Copy();
            alvCaty.SetDataSource(_catyTable);

            alvKs.SetColumnParam.AddQueryColumn("b2ejks");
            alvKs.SetColumnParam.AddQueryColumn("b2ejnm");
            alvKs.SetColumnParam.AddQueryColumn("b2pymx");
            alvKs.SetColumnParam.AddQueryColumn("b2wbmx");
            alvKs.SetColumnParam.AddViewColumn("b2ejks", "编码", 100);
            alvKs.SetColumnParam.AddViewColumn("b2ejnm", "名称", 120);

            alvKs.SetColumnParam.SetTextColumn("b2ejks");
            alvKs.SetColumnParam.SetValueColumn("b2ejks");

            strSql = @"select b2ejks,b2ejnm,b2pymx,b2wbmx from bz02d";

            dsCS = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            alvKs.SetDataSource(dsCS.Copy());

            cbx_scbz.SelectedIndex = 0;
            
            Query();
           
        }

        private kssc_deptinfo GetCurrentDeptInfo()
        {
            kssc_deptinfo deptinfo = new kssc_deptinfo();
            deptinfo.hosp_dept_codg = textBox_hosp_dept_codg.Text;
            deptinfo.hosp_dept_name = textBox_hosp_dept_name.Text;
            deptinfo.caty = textBox_caty.Tag?.ToString() ?? "";
            deptinfo.begntime = dateTimePicker_begntime.Value.ToString("yyyy-MM-dd");
            deptinfo.endtime = "";
            deptinfo.itro = string.IsNullOrEmpty(textBox_itro.Text)?"暂无": textBox_itro.Text;
            deptinfo.dept_resper_name = textBox_dept_resper_name.Text;
            deptinfo.dept_resper_tel = textBox_dept_resper_tel.Text;
            deptinfo.dept_estbdat = dateTimePicker_dept_estbdat.Value.ToString("yyyy-MM-dd");
            deptinfo.hi_crtf_bed_cnt = textBox_hi_crtf_bed_cnt.Text;
            deptinfo.poolarea_no = textBox_poolarea_no.Text;
            deptinfo.aprv_bed_cnt = textBox_aprv_bed_cnt.Text;
            deptinfo.dr_psncnt = textBox_dr_psncnt.Text;
            deptinfo.phar_psncnt = textBox_phar_psncnt.Text;
            deptinfo.nurs_psncnt = textBox_nurs_psncnt.Text;
            deptinfo.tecn_psncnt = textBox_tecn_psncnt.Text;
            deptinfo.memo = textBox_memo.Text;

            return deptinfo;
        }

        private void InsertOrUpdate(int scbz)
        {
            if (string.IsNullOrWhiteSpace(textBox_hosp_dept_codg.Text))
            {
                MyTip.Show("科室代码不能为空", textBox_hosp_dept_codg);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox_hosp_dept_name.Text))
            {
                MyTip.Show("科室名称不能为空", textBox_hosp_dept_name);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox_caty.Tag?.ToString() ?? ""))
            {
                MyTip.Show("科别不能为空", textBox_caty);
                return;
            }
            //if (string.IsNullOrWhiteSpace(textBox_itro.Text))
            //{
            //    MyTip.Show("科室简介不能为空", textBox_itro);
            //    return;
            //}
            if (string.IsNullOrWhiteSpace(textBox_dept_resper_name.Text))
            {
                MyTip.Show("科室负责人姓名不能为空", textBox_dept_resper_name);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox_dept_resper_tel.Text))
            {
                MyTip.Show("科室负责人电话不能为空", textBox_dept_resper_tel);
                return;
            }
            if (!int.TryParse(textBox_aprv_bed_cnt.Text, out int abc) || abc < 0)
            {
                MyTip.Show("批准床位数量不能为空", textBox_aprv_bed_cnt);
                return;
            }
            if (!int.TryParse(textBox_dr_psncnt.Text, out int dpc) || dpc < 0)
            {
                MyTip.Show("医师人数不能为空", textBox_dr_psncnt);
                return;
            }
            if (!int.TryParse(textBox_phar_psncnt.Text, out int ppc) || ppc < 0)
            {
                MyTip.Show("药师人数不能为空", textBox_phar_psncnt);
                return;
            }
            if (!int.TryParse(textBox_nurs_psncnt.Text, out int npc) || npc < 0)
            {
                MyTip.Show("护士人数不能为空", textBox_nurs_psncnt);
                return;
            }
            if (!int.TryParse(textBox_tecn_psncnt.Text, out int tpc) || tpc < 0)
            {
                MyTip.Show("技师人数不能为空", textBox_tecn_psncnt);
                return;
            }

            if (DialogResult.Yes != MessageBox.Show("是否进行上传/变更科室信息?", "", MessageBoxButtons.YesNo))
            {
                return;
            }
            object[] objRet, param = new object[] { textBox_hosp_dept_codg.Text, scbz };
            objRet = Function.ybs_interface("5203", param);
            MessageBox.Show(objRet[2].ToString());
           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            InsertOrUpdate(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_hosp_dept_codg.Text))
            {
                MyTip.Show("科室代码不能为空", textBox_hosp_dept_codg);
                return;
            }
            if (DialogResult.Yes != MessageBox.Show("是否撤销上传的科室信息?", "", MessageBoxButtons.YesNo))
            {
                return;
            }
            object[] objRet, param = new object[] { textBox_hosp_dept_codg.Text };
            objRet = Function.ybs_interface("5205", param);
            MessageBox.Show(objRet[2].ToString());
        }

        private void alvKs_AfterConfirm(object sender, EventArgs e)
        {
            if (alvKs.currentDataRow == null)
            {
                return;
            }
            textBox_hosp_dept_name.Text = alvKs.currentDataRow.Row["b2ejnm"].ToString();

            //string kb = alvKs.currentDataRow.Row["kb"].ToString();
            //textBox_caty.Text = kb;
            //if (_catyTable != null)
            //{
            //    textBox_caty.Text = _catyTable.Tables[0].AsEnumerable().Where(x => x["bzkeyx"].ToString()== kb).Select(x => x["bzname"].ToString())
            //        .FirstOrDefault() ?? "";
            //}
            //textBox_caty.Tag = kb;

            //textBox_aprv_bed_cnt.Text = alvKs.currentDataRow.Row["pzcwsl"].ToString();

            //textBox_itro.Text = alvKs.currentDataRow.Row["itro"].ToString();

            //textBox_poolarea_no.Text = alvKs.currentDataRow.Row["tcqh"].ToString();

            //textBox_dept_resper_name.Text = alvKs.currentDataRow.Row["ksfzrxm"].ToString();

            //textBox_dept_resper_tel.Text = alvKs.currentDataRow.Row["ksfzrdh"].ToString();

            //if (DateTime.TryParse(alvKs.currentDataRow.Row["kssj"].ToString(), out DateTime kssj))
            //{
            //    dateTimePicker_begntime.Value = kssj;
            //}
            //if (DateTime.TryParse(alvKs.currentDataRow.Row["ksclrq"].ToString(), out DateTime ksclrq))
            //{
            //    dateTimePicker_dept_estbdat.Value = ksclrq;
            //}

            //textBox_memo.Text = alvKs.currentDataRow.Row["bz"].ToString();

            //textBox_dept_med_serv_scp.Text = alvKs.currentDataRow.Row["ksylfwfw"].ToString();

            //textBox_dr_psncnt.Text = alvKs.currentDataRow.Row["ysrs"].ToString();
            //textBox_phar_psncnt.Text = alvKs.currentDataRow.Row["jsrs"].ToString();
            //textBox_nurs_psncnt.Text = alvKs.currentDataRow.Row["hsrs"].ToString();
            //textBox_tecn_psncnt.Text = alvKs.currentDataRow.Row["ys1rs"].ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InsertOrUpdate(1);
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query() 
        {
            string strWhere = "";
            if (cbx_scbz.SelectedIndex != 0) 
            {
                if (cbx_scbz.SelectedIndex == 1)
                    strWhere += " where  isnull(d.b2ybup,0)='1'";
                else
                    strWhere += " where  isnull(d.b2ybup,0)='0'";

            }
            string strSql = string.Format(@"with tmp as
                                (
                                SELECT 
                                distinct d.b2ejks 科室编码,
                                d.b2ejnm 科室名称,
                                h.aprv_bed_cnt 批准床位数,
                                h.hi_crtf_bed_cnt 医保认可床位数,
                                h.poolarea_no 统筹区号,
                                h.dr_psncnt 医师人数,
                                h.phar_psncnt 药师人数,
                                h.nurs_psncnt 护士人数,
                                h.tecn_psncnt 技师人数,
                                h.dept_resper_name  科室负责人姓名,
                                h.dept_resper_tel 科室电话,
                                h.begntime 开始时间,
                                h.endtime 结束时间,
                                h.dept_estbdat 科室成立时间,
                                h.b2bjno 科别,
                                h.b2bynm  科别名称
                                 FROM dbo.bz02h h
                                LEFT  JOIN bz02d d ON d.b2ksno=h.b2ksno
                                {1}
                                )
                                select  科室编码,科室名称,批准床位数,医保认可床位数,统筹区号,医师人数,
                                        药师人数,护士人数,技师人数,科室负责人姓名,科室电话,开始时间,
                                        结束时间,科室成立时间,科别,科别名称 from tmp where (科室名称 like '%{0}%' or 科室名称 like '{0}%')", txt_ksno.Text, strWhere);
           DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            gdv_ksxx.DataSource = ds.Tables[0];
        }

        private void gdv_ksxx_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gdv_ksxx.RowCount > 0) 
            {
                
                textBox_hosp_dept_codg.Text = gdv_ksxx.CurrentRow.Cells["科室编码"].Value.ToString();
                textBox_hosp_dept_name.Text = gdv_ksxx.CurrentRow.Cells["科室名称"].Value.ToString();
                textBox_caty.Tag = gdv_ksxx.CurrentRow.Cells["科别"].Value.ToString();
                textBox_caty.Text = gdv_ksxx.CurrentRow.Cells["科别名称"].Value.ToString();
                textBox_poolarea_no.Text = gdv_ksxx.CurrentRow.Cells["统筹区号"].Value.ToString();
                dateTimePicker_dept_estbdat.Value =string.IsNullOrEmpty(gdv_ksxx.CurrentRow.Cells["科室成立时间"].Value.ToString())? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")): DateTime.Parse(gdv_ksxx.CurrentRow.Cells["科室成立时间"].Value.ToString().Substring(0, 4) + "-" + gdv_ksxx.CurrentRow.Cells["科室成立时间"].Value.ToString().Substring(4, 2) + "-" + gdv_ksxx.CurrentRow.Cells["科室成立时间"].Value.ToString().Substring(6, 2));
                dateTimePicker_begntime.Value = string.IsNullOrEmpty(gdv_ksxx.CurrentRow.Cells["开始时间"].Value.ToString()) ? DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")) : DateTime.Parse(gdv_ksxx.CurrentRow.Cells["开始时间"].Value.ToString().Substring(0, 4) + "-" + gdv_ksxx.CurrentRow.Cells["开始时间"].Value.ToString().Substring(4, 2) + "-" + gdv_ksxx.CurrentRow.Cells["开始时间"].Value.ToString().Substring(6, 2));
                textBox_dept_resper_name.Text = gdv_ksxx.CurrentRow.Cells["科室负责人姓名"].Value.ToString();
                textBox_dept_resper_tel.Text = gdv_ksxx.CurrentRow.Cells["科室电话"].Value.ToString();
                textBox_aprv_bed_cnt.Text = gdv_ksxx.CurrentRow.Cells["批准床位数"].Value.ToString();
                textBox_dr_psncnt.Text = gdv_ksxx.CurrentRow.Cells["医师人数"].Value.ToString();
                textBox_phar_psncnt.Text = gdv_ksxx.CurrentRow.Cells["药师人数"].Value.ToString();
                textBox_hi_crtf_bed_cnt.Text = gdv_ksxx.CurrentRow.Cells["医保认可床位数"].Value.ToString();
                textBox_nurs_psncnt.Text = gdv_ksxx.CurrentRow.Cells["护士人数"].Value.ToString();
                textBox_tecn_psncnt.Text = gdv_ksxx.CurrentRow.Cells["技师人数"].Value.ToString();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            object[] objRet, param = new object[] { "", "0" };
            objRet = Function.ybs_interface("5203", param);
                MessageBox.Show(objRet[2].ToString());

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}
