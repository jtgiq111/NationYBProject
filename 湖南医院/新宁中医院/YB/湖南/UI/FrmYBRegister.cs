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

namespace yb_interfaces
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
            string sqlStr = string.Format(@" select bzmem1 dm, bzname dmmc from bztbd where bzcodn='YL' and isnull(bzusex,'0')='1' and bzmem3 like '%m%' ");
            DataSet dsYBBZ = CliUtils.ExecuteSql("sybdj", "cmd", sqlStr, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsYBBZ.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsYBBZ.Tables[0].Rows.Count; i++)
                {
                    mylist.Add(new DictionaryEntry(dsYBBZ.Tables[0].Rows[i]["dm"].ToString(), dsYBBZ.Tables[0].Rows[i]["dmmc"].ToString()));
                }
                cmbYBLB.DataSource = mylist;
                cmbYBLB.DisplayMember = "Value";
                cmbYBLB.ValueMember = "Key";
                cmbYBLB.SelectedIndex = 0;
            }


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
            this.dt.Columns.Add("医生姓名", typeof(string));
            this.dt.Columns.Add("发票号", typeof(string));
            this.dt.Columns.Add("执行科室", typeof(string));
            this.dt.Columns.Add("处方流水号", typeof(string));
            this.dt.Columns.Add("处方号", typeof(string));
            this.dgvsfmx.DataSource = dt;
            this.dgvsfmx.ReadOnly = true;
            this.dgvsfmx.RowHeadersVisible = false;
            this.dgvsfmx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvsfmx.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvsfmx.AllowUserToAddRows = false;
            this.dgvsfmx.Columns["处方号"].Width = 250;

            ///初始化病种
            ///
            DataTable dtbz = new DataTable();
            string sql = "select dm,dmmc,pym,wbm from ybbzmrdr";
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
                string sqlStr = string.Format(@"select * from mz01h h where h.m1ghno='{0}' and left(m1date,10)=Convert(varchar(10),getdate(),120)", clinicNO);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sqlStr, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("找不到该挂号流水号的挂号信息或挂号日期不是当天！！(ಥ﹏ಥ)");
                    return;
                }
                dsinfo = ds;
                string name = ds.Tables[0].Rows[0]["m1name"].ToString();
                string sex = ds.Tables[0].Rows[0]["m1sexx"].ToString();
                DateTime brith = DateTime.Parse(ds.Tables[0].Rows[0]["m1bird"].ToString());
                string age = ds.Tables[0].Rows[0]["m1agex"].ToString();
                DateTime djrq = DateTime.Parse(ds.Tables[0].Rows[0]["m1date"].ToString());
                cmbJFLX.Text = ds.Tables[0].Rows[0]["m1kdnm"].ToString();
                this.txtName.Text = name;
                this.txtGender.Text = sex;
                this.txtAge.Text = age;
                this.txtBrith.Text = brith.ToString("yyyy-MM-dd");
                this.txtDjrq.Text = djrq.ToString("yyyy-MM-dd");
                //绑定发票号的下拉框
                string strSql1 = string.Format(@"select b.ybxmbh,b.ybxmmc,a.m4pric as dj,
                                        sum(a.m4quty) as sl,
                                        sum(a.m4amnt) as je,
                                        a.m4item as yyxmbh, a.m4name as yyxmmc,max(a.m4date) as yysj, m4sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.m4empn as ysbh, b1name ysxm,
                                        a.m4invo as invono,a.m4zxks as zxks,a.m4cfno+a.m4sequ as cflsh,a.m4cfno as cfno
                                        from mz04d a left join ybhisdzdrnew b on a.m4item=b.hisxmbh 
                                        left join bz01h c on b1empn=m4empn
                                        where a.m4ghno='{0}' and m4endv not like '4%'
                                        group by b.ybxmbh,b.ybxmmc,a.m4pric,a.m4item,a.m4name,a.m4sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,m4empn,a.m4invo,a.m4zxks,a.m4cfno+a.m4sequ,a.m4cfno,b1name
                                        having sum(a.m4amnt)>0  ", clinicNO );
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
            string yllb = this.cmbYBLB.SelectedValue.ToString();

            string clinicNO = this.txtClinicId.Text.Trim();
            string doctCode = dsinfo.Tables[0].Rows[0]["m1empn"].ToString();
            DateTime djrq = DateTime.Parse(dsinfo.Tables[0].Rows[0]["m1date"].ToString());
            //医保挂号
            //, yllb, bzbm, bzmc, jssj, dgysdm, dgysxm
            object[] res = yb_interface_hn_nkNew.YBMZDK(null);
            if (!res[1].ToString().Equals("1"))
            {
                MessageBox.Show(res[2].ToString());
                return;
            }
            object[] req = new object[] { clinicNO, yllb, bzbm, bzmc, res[2].ToString() ,djrq.ToString("yyyy-MM-dd HH:mm:ss")};
            res = yb_interface_hn_nkNew.YBMZDJ(req);
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
            if (this.dgvsfmx.Rows.Count == 0)
            {
                MessageBox.Show("该患者没有任何费用！(*^▽^*)");
                return;
            }

            for (int i = 0; i < dgvsfmx.Rows.Count; i++)
            {
                je += Decimal.Parse(dgvsfmx.Rows[i].Cells["金额"].Value.ToString());

                if (i == dgvsfmx.Rows.Count - 1)
                {
                    cfhs1 += "'" + dgvsfmx.Rows[i].Cells["处方号"].Value.ToString() + "'";
                }
                else
                {
                    cfhs1 += "'" + dgvsfmx.Rows[i].Cells["处方号"].Value.ToString() + "',";

                }

            }
            string doctCode = this.dgvsfmx.Rows[0].Cells["医生编号"].Value.ToString();
            string doctName = this.dgvsfmx.Rows[0].Cells["医生姓名"].Value.ToString();

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
            object[] objres = new object[] { jzlsh, "0", jssj, bzbm, bzmc, cfhs, yllb, sfje, cfysdm, cfysxm };
            object[] ii = yb_interface_hn_nkNew.YBMZSFYJS(objres);
            if (ii[1].ToString() == "1")
            {
                string yjsRet = "医疗费总额" + ii[2].ToString().Split('|')[0] + "\r\n" +
                    "总报销金额" + ii[2].ToString().Split('|')[1] + "\r\n" +
                    "统筹基金支付" + ii[2].ToString().Split('|')[2] + "\r\n" +
                    "大额基金支付" + ii[2].ToString().Split('|')[3] + "\r\n" +
                    "账户支付" + ii[2].ToString().Split('|')[4] + "\r\n" +
                    "现金支付" + ii[2].ToString().Split('|')[5] + "\r\n" +
                    "公务员补助基金支付" + ii[2].ToString().Split('|')[6] + "\r\n" +
                    "企业补充医疗保险基金支付" + ii[2].ToString().Split('|')[7] + "\r\n" +
                    "医院负担费用" + ii[2].ToString().Split('|')[10] + "\r\n" +
                    "民政救助费用" + ii[2].ToString().Split('|')[11] + "\r\n";
                DialogResult result = MessageBox.Show("确认进行医保结算吗？\r\n" + yjsRet, "Waning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    return;
                object[] objresjs = new object[] { jzlsh, invoiceNo, "0", jssj, bzbm, bzmc, cfhs, yllb, sfje, cfysdm, cfysxm };
                object[] i1 = yb_interface_hn_nkNew.YBMZSFJS(objresjs);
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

            string strSql2 = string.Format(@"select b.ybxmbh,b.ybxmmc,a.m4pric as dj,
                                        sum(a.m4quty) as sl,
                                        sum(a.m4amnt) as je,
                                        a.m4item as yyxmbh, a.m4name as yyxmmc,max(a.m4date) as yysj, m4sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.m4empn as ysbh, b1name ysxm,
                                        a.m4invo as invono,a.m4zxks as zxks,a.m4cfno+a.m4sequ as cflsh,a.m4cfno as cfno
                                        from mz04d a left join ybhisdzdrnew b on a.m4item=b.hisxmbh 
                                        left join bz01h c on b1empn=m4empn
                                        where a.m4ghno='{0}' and m4invo='{1}' and m4endv not like '4%'
                                        group by b.ybxmbh,b.ybxmmc,a.m4pric,a.m4item,a.m4name,a.m4sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,m4empn,a.m4invo,a.m4zxks,a.m4cfno+a.m4sequ,a.m4cfno,b1name
                                        having sum(a.m4amnt)>0 ", jzlsh, invoiceNo);
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
                newRow["医生姓名"] = item["ysxm"].ToString();
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
            string filterStr = string.Format(@"ICD编码 like '%{0}%' or ICD名称 like '%{0}%' or 拼音码 like '%{0}%' or 五笔码 like '%{0}%' or 'ALL'='{0}' ", rowWord);
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

        private void label3_Click(object sender, EventArgs e)
        {
        
        }
    }
}
