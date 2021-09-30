using Srvtools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class FrmYBRegister : InfoForm
    {

        /// <summary>
        /// 门诊医保登记结算补办
        /// </summary>
        public FrmYBRegister()
        {
            InitializeComponent();
            Init();
        }
        /// <summary>
        /// 费用sql
        /// </summary>
        public string strSql = @"select b.ybxmbh,b.ybxmmc,a.m4pric as dj,
                                        sum(a.m4quty) as sl,
                                        sum(a.m4amnt) as je,
                                        a.m4item as yyxmbh, a.m4name as yyxmmc,max(a.m4date) as yysj, m4sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.m4empn as ysbh,
                                        a.m4rinvo as invono,a.m4zxks as zxks,a.m4cfno+a.m4sequ as cflsh,a.m4cfno as cfno
                                        from mz04d a left join ybhisdzdr b on 
                                        a.m4item=b.hisxmbh where   a.m4ghno='{0}' and b.ybxmbh not in ('001101000010000','001102000010000')  and (a.m4cfno in ({1}) or 'ALL'={1}) and (a.m4rinvo='{2}' or 'ALL'='{2}')
                                        group by b.ybxmbh,b.ybxmmc,a.m4pric,a.m4item,a.m4name,a.m4sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,m4empn,a.m4rinvo,a.m4zxks,a.m4cfno+a.m4sequ,a.m4cfno
                                        having sum(a.m4amnt)>0
                                        union all
                                        select b.ybxmbh,b.ybxmmc,a.mcpric as dj,
                                        sum(a.mcquty) as sl,
                                        sum(a.mcamnt) as je,
                                        a.mcypno as yyxmbh, a.mcypnm as yyxmmc,max(a.mcdate) as yysj, mcpchx as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.mcuser as ysbh,
                                        a.mcinvo as invono,a.mcksn2 as zxks,a.mccfno+a.mcseq1+mcseq2 as cflsh,a.mccfno as cfno
                                        from mzcfd a left join ybhisdzdr b on 
                                        a.mcypno=b.hisxmbh where   a.mcghno='{0}' and b.ybxmbh not in ('001101000010000','001102000010000')  and( a.mccfno in ({1})  or 'ALL'={1})  and (a.mcinvo='{2}' or 'ALL'='{2}')
                                        group by b.ybxmbh,b.ybxmmc,a.mcpric,a.mcypno,a.mcypnm,a.mcpchx,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,mcuser,a.mcinvo,a.mcksn2,a.mccfno+a.mcseq1+mcseq2,a.mccfno
                                        having sum(a.mcamnt)>0 
                                        union all
                                        select 
                                        b.ybxmbh,b.ybxmmc,c.mbpric as dj,
                                        c.mbquty as sl,
                                        sum(c.mbsjam) as je,
                                        c.mbitem as yyxmbh, c.mbname as yyxmmc,max(c.mbdate) as yysj, mbsfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mbuser as ysbh,
                                        c.mbinvo as invono,c.mbzxks as zxks,c.mbzlno+c.mbsequ as cflsh,c.mbzlno as cfno
                                        from mzb4d  c  left join ybhisdzdr b on c.mbitem=b.hisxmbh 
                                        where c.mbghno='{0}' and (c.mbzlno in ({1}) or 'ALL'={1}) and (c.mbinvo='{2}' or 'ALL'='{2}')
                                        group by b.ybxmbh,b.ybxmmc,c.mbpric,c.mbitem,c.mbname,c.mbsfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,c.mbuser ,c.mbquty,c.mbinvo,c.mbzxks,c.mbzlno+c.mbsequ,c.mbzlno
                                         union all
                                        select 
                                        b.ybxmbh,b.ybxmmc,c.mbpric as dj,
                                        c.mbquty as sl,
                                        sum(c.mbsjam) as je,
                                        c.mbitem as yyxmbh, c.mbname as yyxmmc,max(c.mbdate) as yysj, mbsfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mbuser as ysbh,
                                        c.mbinvo as invono,c.mbzxks as zxks,c.mbzlno+c.mbsequ as cflsh,c.mbzlno as cfno
                                        from mzb2d c  left join ybhisdzdr b on c.mbitem=b.hisxmbh 
                                        where c.mbghno='{0}' and (c.mbzlno in ({1}) or 'ALL'={1}) and (c.mbinvo='{2}' or 'ALL'='{2}')
                                        group by b.ybxmbh,b.ybxmmc,c.mbpric,c.mbitem,c.mbname,c.mbsfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,c.mbuser,c.mbquty,c.mbinvo,c.mbzxks,c.mbzlno+c.mbsequ,c.mbzlno
                                    union all
                                        select 
                                        b.ybxmbh,b.ybxmmc,c.mdpric as dj,
                                        c.mdpqty as sl,
                                        sum(c.mdamnt) as je,
                                        c.mdhsdm as yyxmbh, c.mdhsmc as yyxmmc,max(c.mddate) as yysj, c.mdcfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mdempn as ysbh,
                                        c.mdinvo as invono,c.mdzsks as zxks,c.mdcfno+c.mdseq1 as cflsh,c.mdcfno as cfno
                                        from mzd3d c  left join ybhisdzdr b on c.mdhsdm=b.hisxmbh 
                                        where c.mdghno='{0}' and (c.mdcfno in({1}) or 'ALL'={1}) and (c.mdinvo='{2}' or 'ALL'='{2}')
                                        group by b.ybxmbh,b.ybxmmc,c.mdpric,c.mdhsdm,c.mdhsmc,c.mdcfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,c.mdempn,c.mdpqty,c.mdinvo,c.mdzsks,c.mdcfno+c.mdseq1,c.mdcfno";
        /// <summary>
        /// 诊断sql
        /// </summary>
        public string zdSql = "select a.* from mza1dd a where a.m1ghno='{0}'";
        public DataTable dt = new DataTable();

        public DataTable dt1 = new DataTable();
        public void Init()
        {
            ///医疗类别
            this.cmbYBLB.Enabled = true;
            ArrayList mylist = new ArrayList();
            mylist.Add(new DictionaryEntry("11", "11|门诊肢体残"));
            mylist.Add(new DictionaryEntry("12", "12|门诊职业病"));
            mylist.Add(new DictionaryEntry("13", "13|门诊康复"));
            cmbYBLB.DataSource = mylist;
            cmbYBLB.DisplayMember = "Key";
            cmbYBLB.ValueMember = "Value";
            cmbYBLB.SelectedIndex = 0;

            ///缴费类型
            this.cmbJFLX.Enabled = true;
            ArrayList arrlist = new ArrayList();
            arrlist.Add(new DictionaryEntry("0001", "居民医保"));
            arrlist.Add(new DictionaryEntry("0002", "职工医保"));
            arrlist.Add(new DictionaryEntry("0003", "工伤医保"));
            arrlist.Add(new DictionaryEntry("0004", "本院职工"));
            arrlist.Add(new DictionaryEntry("0005", "外地工伤"));
            arrlist.Add(new DictionaryEntry("0006", "省医保"));
            arrlist.Add(new DictionaryEntry("0007", "本院离退"));
            arrlist.Add(new DictionaryEntry("0008", "离休"));
            arrlist.Add(new DictionaryEntry("0009", "公费医疗"));
            arrlist.Add(new DictionaryEntry("0010", "外地医保"));
            this.cmbJFLX.DataSource = arrlist;
            this.cmbJFLX.DisplayMember = "Key";
            this.cmbJFLX.ValueMember = "Value";
            this.cmbJFLX.SelectedIndex = 0;


            ///初始化发票下拉框
            List<string> strlist = new List<string>() { "请选择发票号" };
            this.cmbInvoice.Items.AddRange(strlist.ToArray());
            this.cmbInvoice.Text = "请选择发票号";

            ///初始化datagridview
            ///
            this.dt.Columns.Add("医保机构项目编码", typeof(string));
            this.dt.Columns.Add("医保机构项目名称", typeof(string));
            this.dt.Columns.Add("医院项目编码", typeof(string));
            this.dt.Columns.Add("医院项目名称", typeof(string));
            this.dt.Columns.Add("单价", typeof(string));
            this.dt.Columns.Add("数量", typeof(string));
            this.dt.Columns.Add("金额", typeof(string));
            this.dt.Columns.Add("结算时间", typeof(string));
            this.dt.Columns.Add("收费序号", typeof(string));
            this.dt.Columns.Add("项目诊疗编码", typeof(string));
            this.dt.Columns.Add("项目类别代码", typeof(string));
            this.dt.Columns.Add("项目类型", typeof(string));
            this.dt.Columns.Add("医生编号", typeof(string));
            this.dt.Columns.Add("发票号", typeof(string));
            this.dt.Columns.Add("执行科室", typeof(string));
            this.dt.Columns.Add("处方流水号", typeof(string));
            this.dt.Columns.Add("处方号", typeof(string));
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Columns["处方号"].Width = 250;

            ///初始化病种
            ///
            DataTable dtbz = new DataTable();
            string sql = "select *from ybbzmrdr";
            dtbz = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject) == null ? null : CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            if (dtbz == null)
            {
                MessageBox.Show("初始化诊断字典失败！o(╥﹏╥)o");
            }
            else
            {
                ///初始化诊断字典
                ///
                dt1.Columns.Add("ICD编码", typeof(string));
                dt1.Columns.Add("ICD名称", typeof(string));
                dt1.Columns.Add("拼音码", typeof(string));
                dt1.Columns.Add("五笔码", typeof(string));
                string[] icdpym = { "ABC", "BCD", "CDE", "DEF", "EFG" };
                string[] icdwbm = { "GB", "GBE", "GBF", "FBA", "FB" };
                for (int i = 0; i < dtbz.Rows.Count; i++)
                {
                    DataRow dr = dt1.NewRow();
                    dr["ICD编码"] = dtbz.Rows[i]["dm"].ToString();
                    dr["ICD名称"] = dtbz.Rows[i]["dmmc"].ToString();
                    dr["拼音码"] = dtbz.Rows[i]["pym"].ToString();
                    dr["五笔码"] = dtbz.Rows[i]["wbm"].ToString();
                    dt1.Rows.Add(dr);
                }
                this.dgvIcd.DataSource = dt1;
                this.dgvIcd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
                this.dgvIcd.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                this.dgvIcd.AllowUserToAddRows = false;
                this.dgvIcd.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            this.dgvIcd.Visible = false;

        }

        private void txtClinicId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(this.txtClinicId.Text.Trim()))
                {
                    MessageBox.Show("挂号流水号不得为空！！(ಥ﹏ಥ)");
                    return;
                }
                string clinicNO = this.txtClinicId.Text.Trim().PadLeft(9, '0');
                this.txtClinicId.Text = clinicNO;
                string sqlStr = $"select *from mz01h h where h.m1ghno='{clinicNO}' and (h.m1ybno ='' or h.m1ybno is null)";
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sqlStr, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("找不到该挂号流水号的挂号信息！！(ಥ﹏ಥ)");
                    return;
                }
                dsinfo = ds;
                string name = ds.Tables[0].Rows[0]["m1name"].ToString();
                string sex = ds.Tables[0].Rows[0]["m1sexx"].ToString();
                DateTime brith = DateTime.Parse(ds.Tables[0].Rows[0]["m1bird"].ToString());
                string age = ds.Tables[0].Rows[0]["m1agex"].ToString();
                DateTime djrq = DateTime.Parse(ds.Tables[0].Rows[0]["m1date"].ToString());
                this.txtName.Text = name;
                this.txtGender.Text = sex;
                this.txtAge.Text = age;
                this.txtBrith.Text = brith.ToString("yyyy-MM-dd");
                this.txtDjrq.Text = djrq.ToString("yyyy-MM-dd");
                //绑定发票号的下拉框
                string strSql1 = string.Format(strSql, clinicNO, "'ALL'", "ALL");
                DataSet dsinvo = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dsinvo.Tables[0].Rows.Count <= 0)
                {
                    return;
                }
                List<string> invoList = new List<string>();
                invoList = (from DataRow r in dsinvo.Tables[0].AsEnumerable()
                            where !string.IsNullOrEmpty(r.Field<string>("invono"))
                            select r.Field<string>("invono")).Distinct().ToList();
                if (invoList.Count <= 0)
                {
                    MessageBox.Show("该患者没有已结算的发票信息");
                    return;
                }
                this.cmbInvoice.Items.Clear();
                this.cmbInvoice.Items.AddRange(invoList.ToArray());
                this.cmbInvoice.SelectedIndex = 0;
            }
        }
        DataSet dsinfo = new DataSet();
        private void button1_Click(object sender, EventArgs e)
        {
            string yllb = this.cmbYBLB.SelectedValue.ToString().Split('|')[0];

            string clinicNO = this.txtClinicId.Text.Trim();
            string doctCode = dsinfo.Tables[0].Rows[0]["m1empn"].ToString();
            DateTime djrq = DateTime.Parse(dsinfo.Tables[0].Rows[0]["m1date"].ToString());
            //医保挂号
            //, yllb, bzbm, bzmc, jssj, dgysdm, dgysxm
            object[] req = new object[] { clinicNO, yllb, "", "", djrq.ToString("yyyy-MM-dd HH:mm:ss"), "", doctCode };
            object[] res = yb_interface_sy_zrNew.YBMZDJSFBL(req);
            if (res[1].ToString() == "1")
            {
                MessageBox.Show("医保挂号登记成功！(*^▽^*)");
            }
            else
            {

                MessageBox.Show(res[2].ToString() + "o(╥﹏╥)o");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string jzlsh = this.txtClinicId.Text.Trim();
            if (string.IsNullOrEmpty(jzlsh))
            {
                MessageBox.Show("门诊流水号不得为空!!！o(╥﹏╥)o");
                return;
            }
            decimal je = 0m;
            string cfhs1 = string.Empty;
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("该患者没有任何费用！(*^▽^*)");
                return;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                je += Decimal.Parse(dataGridView1.Rows[i].Cells["金额"].Value.ToString());

                if (i == dataGridView1.Rows.Count - 1)
                {
                    cfhs1 += "'" + dataGridView1.Rows[i].Cells["处方号"].Value.ToString() + "'";
                }
                else
                {
                    cfhs1 += "'" + dataGridView1.Rows[i].Cells["处方号"].Value.ToString() + "',";

                }

            }
            string doctCode = this.dataGridView1.Rows[0].Cells["医生编号"].Value.ToString();
            string doctName = yb_interface_sy_zrNew.GetEmplNameById(doctCode);

            string sql = "";

            string jssj = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string icdcode = string.Empty;//病种编号
            string icdName = string.Empty;//病种名称
            if (string.IsNullOrEmpty(this.txtIcd.Text.Trim()))
            {
                string sqlzd = string.Format(zdSql, jzlsh);
                DataTable dtzd = CliUtils.ExecuteSql("sybdj", "cmd", sqlzd, CliUtils.fLoginDB, true, CliUtils.fCurrentProject) == null ? null : CliUtils.ExecuteSql("sybdj", "cmd", sqlzd, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                if (dtzd == null)
                {
                    MessageBox.Show("诊断查询失败！！o(╥﹏╥)o");
                    return;
                }
                icdcode = dtzd.Rows[0]["m1xyzd"].ToString();
                icdName = dtzd.Rows[0]["m1xynm"].ToString();

            }
            else
            {
                icdcode = bzbm;
                icdName = bzmc;
            }
            string invoiceNo = this.cmbInvoice.SelectedItem.ToString();//单据号
            string cfhs = cfhs1;   //处方号集
            string yllb = this.cmbYBLB.SelectedValue.ToString().Split('|')[0];   //医疗类别
            string sfje = je.ToString();   //收费金额
            string cfysdm = doctCode; //处方医生代码
            string cfysxm = doctName; //处方医生姓名

            //医保接口调用
            object[] objres = new object[] { jzlsh, "", jssj, bzbm, bzmc, cfhs, yllb, sfje, cfysdm, cfysxm };
            object[] ii = yb_interface_sy_zrNew.YBMZSFYJS1(objres);
            if (ii[1].ToString() == "1")
            {
                object[] objresjs = new object[] { jzlsh, invoiceNo, "", jssj, bzbm, bzmc, cfhs, yllb, sfje, cfysdm, cfysxm };
                object[] i1 = yb_interface_sy_zrNew.YBMZSFJS(objresjs);
                if (i1[1].ToString() == "1")
                {
                    MessageBox.Show("医保门诊结算成功！" + "(*^▽^*)!");
                }
                else
                {
                    MessageBox.Show(i1[2].ToString() + "o(╥﹏╥)o！");
                }
            }
            else
            {
                MessageBox.Show(ii[2].ToString() + "o(╥﹏╥)o！");
            }

        }

        private void cmbInvoice_SelectedValueChanged(object sender, EventArgs e)
        {
            string jzlsh = this.txtClinicId.Text.Trim();
            string invoiceNo = this.cmbInvoice.SelectedItem.ToString();
            if (string.IsNullOrEmpty(jzlsh))
            {
                //   MessageBox.Show("就诊流水号不得为空！！！[○･｀Д´･ ○]");
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(invoiceNo) || invoiceNo == "请选择发票号")
                {
                    MessageBox.Show("请选择有效的发票号！！！[○･｀Д´･ ○]");
                    return;
                }
            }

            string strSql2 = string.Format(strSql, jzlsh, "'ALL'", invoiceNo);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql2, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count <= 0)
            {
                MessageBox.Show("该发票号没有找到费用明细！！！[○･｀Д´･ ○]");
                return;
            }

            List<DataRow> drlist = (from DataRow r in ds.Tables[0].AsEnumerable()
                                    where !string.IsNullOrEmpty(r.Field<string>("invono"))
                                    select r).ToList();
            dt.Rows.Clear();
            foreach (DataRow item in drlist)
            {
                DataRow newRow = dt.NewRow();
                newRow["医保机构项目编码"] = item["ybxmbh"].ToString();
                newRow["医保机构项目名称"] = item["ybxmmc"].ToString();
                newRow["医院项目编码"] = item["yyxmbh"].ToString();
                newRow["医院项目名称"] = item["yyxmmc"].ToString();
                newRow["单价"] = item["dj"].ToString();
                newRow["数量"] = item["sl"].ToString();
                newRow["金额"] = item["je"].ToString();
                newRow["结算时间"] = item["yysj"].ToString();
                newRow["收费序号"] = item["sfno"].ToString();
                newRow["项目诊疗编码"] = item["ybsfxmzldm"].ToString();
                newRow["项目类别代码"] = item["ybsflbdm"].ToString();
                newRow["项目类型"] = item["xmlx"].ToString();
                newRow["医生编号"] = item["ysbh"].ToString();
                newRow["发票号"] = item["invono"].ToString();
                newRow["执行科室"] = item["zxks"].ToString();
                newRow["处方流水号"] = item["cflsh"].ToString();
                newRow["处方号"] = item["cfno"].ToString();
                this.dt.Rows.Add(newRow);
            }

        }

        private void FrmYBRegister_Load(object sender, EventArgs e)
        {
            // TODO: 这行代码将数据加载到表“bzinfo.ybbzmrdr”中。您可以根据需要移动或删除它。
            //   this.ybbzmrdrTableAdapter.Fill(this.bzinfo.ybbzmrdr);

        }

        private void txtIcd_TextChanged(object sender, EventArgs e)
        {
            string rowFilter = txtIcd.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(rowFilter))
            {
                rowFilter = "ALL";
            }
            Filterdt(rowFilter);
        }

        private void Filterdt(string rowWord)
        {
            /*
             dr["ICD编码"] = i + 1;
                dr["ICD名称"] = "病种" + (i + 1);
                dr["拼音码"] = icdpym[i];
                dr["五笔码"] = icdwbm[i];
             */

            DataView dv = dt1.DefaultView;
            string filterStr = string.Format($"ICD编码 like '%{rowWord}%' or ICD名称 like '%{rowWord}%' or 拼音码 like '%{rowWord}%' or 五笔码 like '%{rowWord}%' or 'ALL'='{rowWord}'");
            dv.RowFilter = filterStr;
            DataTable dtres = dv.ToTable();
            this.dgvIcd.DataSource = dtres;
        }

        private void txtIcd_Click(object sender, EventArgs e)
        {
            this.dgvIcd.Visible = true;
            string rowFilter = txtIcd.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(rowFilter))
            {
                rowFilter = "ALL";
            }
            Filterdt(rowFilter);
        }
        public string bzbm = string.Empty;
        public string bzmc = string.Empty;
        private void dgvIcd_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string icdCode = this.dgvIcd.Rows[e.RowIndex].Cells["ICD编码"].Value.ToString();
            string icdName = this.dgvIcd.Rows[e.RowIndex].Cells["ICD名称"].Value.ToString();
            bzbm = icdCode;
            bzmc = icdName;
            this.txtIcd.Text = bzmc;
            this.dgvIcd.Visible = false;
        }
        private void txtIcd_LostFocus(object sender, EventArgs e)
        {
            this.dgvIcd.Visible = false;
        }
    }
}
