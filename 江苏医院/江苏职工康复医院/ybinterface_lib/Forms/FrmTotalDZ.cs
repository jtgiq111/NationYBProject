using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class FrmTotalDZ : InfoForm
    {
        public FrmTotalDZ()
        {
            InitializeComponent();
            InitDs();
        }

        /// <summary>
        /// 总账
        /// </summary>
        private DataSet dsTotal = new DataSet();
        /// <summary>
        /// 明细查询
        /// </summary>
        private DataSet dsDetail = new DataSet();
        /// <summary>
        /// 对账结果
        /// </summary>
        private DataSet dsResult = new DataSet();



        private void initDgv(DataGridView gridView)
        {
            gridView.BorderStyle = BorderStyle.FixedSingle;
            gridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.RowHeadersVisible = false;
            gridView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            gridView.ReadOnly = true;
            gridView.AllowUserToAddRows = false;

            for (int i = 0; i < gridView.Rows.Count; i++)
            {
                //dgvData.Rows[i].Height = 500;
                gridView.Rows[i].ReadOnly = true;
                //for (int j = 0; j < dgvData.Columns.Count; j++)
                //{
                //    dgvData.Rows[i].Cells[j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //}
            }
        }
        /// <summary>
        /// 初始化表格
        /// </summary>
        /// <returns></returns>
        public int InitDs()
        {
            #region 汇总查询
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            #region 绑定列
            ds.Tables[0].Columns.Add("His", typeof(string));
            ds.Tables[0].Columns.Add("金额(His)", typeof(string));
            ds.Tables[0].Columns.Add("医保", typeof(string));
            ds.Tables[0].Columns.Add("金额(医保)", typeof(string));

            #endregion
            List<string> itemNameList = new List<string>()
            {
                "医疗总金额","统筹支付金额","现金支付总额"
            };
            for (int i = 0; i < itemNameList.Count; i++)
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr[0] = "HIS" + itemNameList[i];
                dr[1] = "0";
                dr[2] = "医保" + itemNameList[i];
                dr[3] = "1.2";
                ds.Tables[0].Rows.Add(dr);
            }
            DataRow row = ds.Tables[0].NewRow();
            row[0] = "医保对账结果：";
            row[1] = "无";
            ds.Tables[0].Rows.Add(row);
            this.dsTotal = ds;
            this.dgvData.DataSource = this.dsTotal.Tables[0];
            initDgv(this.dgvData);
            this.dgvData.Rows[3].Cells[1].Style = new DataGridViewCellStyle() { ForeColor = Color.Red };


            #endregion

            #region 明细查询

            #region His明细查询
            DataSet dsdz = new DataSet();
            dsdz.Tables.Add(new DataTable());
            dsdz.Tables[0].Columns.Add("收费员", typeof(string));
            dsdz.Tables[0].Columns.Add("门诊住院流水号", typeof(string));
            dsdz.Tables[0].Columns.Add("发票号", typeof(string));
            dsdz.Tables[0].Columns.Add("his医疗总金额", typeof(decimal));
            dsdz.Tables[0].Columns.Add("his统筹支付金额", typeof(decimal));
            dsdz.Tables[0].Columns.Add("his现金支付金额", typeof(decimal));
            dsdz.Tables[0].Columns.Add("医保医疗总金额", typeof(decimal));
            dsdz.Tables[0].Columns.Add("医保统筹支付金额", typeof(decimal));
            dsdz.Tables[0].Columns.Add("医保现金支付金额", typeof(decimal));
            DataRow dr1 = dsdz.Tables[0].NewRow();
            dr1["收费员"] = "001";
            dr1["门诊住院流水号"] = "testclinic001";
            dr1["发票号"] = "1111";
            dr1["his医疗总金额"] = 0;
            dr1["his统筹支付金额"] = 0;
            dr1["his现金支付金额"] = 0;
            dr1["医保医疗总金额"] = 1.2;
            dr1["医保统筹支付金额"] = 1.2;
            dr1["医保现金支付金额"] = 1.2;
            dsdz.Tables[0].Rows.Add(dr1);
            this.dsDetail = dsdz;
            this.dgvDetail.DataSource = this.dsDetail.Tables[0];
            initDgv(this.dgvDetail);
            #endregion

            #region 对账结果查询
            DataSet dsres = new DataSet();
            dsres.Tables.Add(new DataTable());
            dsres.Tables[0].Columns.Add("收费员", typeof(string));
            dsres.Tables[0].Columns.Add("门诊住院流水号", typeof(string));
            dsres.Tables[0].Columns.Add("发票号", typeof(string));
            dsres.Tables[0].Columns.Add("his医疗总金额", typeof(decimal));
            dsres.Tables[0].Columns.Add("his统筹支付金额", typeof(decimal));
            dsres.Tables[0].Columns.Add("his现金支付金额", typeof(decimal));
            dsres.Tables[0].Columns.Add("医保医疗总金额", typeof(decimal));
            dsres.Tables[0].Columns.Add("医保统筹支付金额", typeof(decimal));
            dsres.Tables[0].Columns.Add("医保现金支付金额", typeof(decimal));
            dsres.Tables[0].Columns.Add("对账结果", typeof(string));

            this.dsResult = dsres;
            this.dgvResult.DataSource = this.dsResult.Tables[0];
            initDgv(this.dgvResult);
            #endregion

            #endregion

            return 1;
        }
        private static string UserId = string.Empty;
        private void btnSel_Click(object sender, EventArgs e)
        {
            int tabIndex = this.tabControl1.SelectedIndex;
            DateTime start = DateTime.Parse(dtStart.Value.ToString("yyyy-MM-dd 00:00:00"));
            DateTime end = DateTime.Parse(dtEnd.Value.ToString("yyyy-MM-dd 23:59:59"));
            string userId = this.radAll.Checked ? "ALL" : CliUtils.fLoginUser;
            UserId = userId;
            object[] objlist = new object[] { };
            string beginTime = start.ToString("yyyy-MM-dd 00:00:00");
            string EndTime = end.ToString("yyyy-MM-dd 23:59:59");
            if (tabIndex == 0)//查询总账
            {
                #region 汇总绑定值
                string StrSql = @"select sum(isnull(y.ylfze,0)) as ylfze,sum(isnull(y.tcjjzf,0)) as tczfje,sum(isnull(y.xjzf,0)) as xjzfje from  ybfyjsdr y where y.sysdate>='{0}' and (y.jbr='{2}' or 'ALL'='{2}')  and y.sysdate<'{1}' and y.isgs='1' and y.cxbz='1'";
                StrSql = string.Format(StrSql, beginTime, EndTime, userId);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", StrSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("没有找到相关的费用信息");
                    return;
                }
                userId = userId == "ALL" ? "" : userId;
                //医保返回
                (int code, string result) = yb_interface_sy_zrNew.TotalDZ(start.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss"), ref objlist,opercode:userId);
                if (code <= 0)
                {
                    MessageBox.Show("调用医保接口出现异常" + result);
                    return;
                }
                string totalcost = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ylfze"].ToString()) ? "0" : ds.Tables[0].Rows[0]["ylfze"].ToString();
                string pubcost = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["tczfje"].ToString()) ? "0" : ds.Tables[0].Rows[0]["tczfje"].ToString();
                string owncost = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["xjzfje"].ToString()) ? "0" : ds.Tables[0].Rows[0]["xjzfje"].ToString();
                dgvData.Rows[0].Cells[1].Value = totalcost;//his医疗总金额
                dgvData.Rows[0].Cells[3].Value = objlist[0];//医保医疗总金额

                dgvData.Rows[1].Cells[1].Value = pubcost;//his统筹支付金额
                dgvData.Rows[1].Cells[3].Value = objlist[1];//医保统筹支付金额

                dgvData.Rows[2].Cells[1].Value = owncost;//his现金支付金额
                dgvData.Rows[2].Cells[3].Value = objlist[2];//医保现金支付金额 
                #endregion
                this.dgvData.Rows[3].Cells[1].ValueType = typeof(string);
                this.dgvData.Rows[3].Cells[1].Style.ForeColor = Color.Green;
                this.dgvData.Rows[3].Cells[1].Value = "无结果";
            }
            else if (tabIndex == 1)
            {
                #region 明细绑定值
                string StrSqlmx = @"select y.jzlsh as jzlsh,y.djhin as djh,sum(isnull(y.ylfze,0)) as ylfze,sum(isnull(y.tcjjzf,0)) as tczfje,sum(isnull(y.xjzf,0)) as xjzfje,y.jbr as jbr from  ybfyjsdr y where y.sysdate>='{0}' and y.sysdate<'{1}' and y.isgs='1' and (y.jbr='{2}' or 'ALL'='{2}')  group by y.jzlsh,y.djhin,y.jbr";//and y.isgs='1'
                StrSqlmx = string.Format(StrSqlmx, beginTime, EndTime, userId);

                DataSet dsmx = CliUtils.ExecuteSql("sybdj", "cmd", StrSqlmx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dsmx.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("查询费用无数据！");
                    return;
                } 
                this.dsDetail.Tables[0].Rows.Clear();
                #region 同步方法
                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    DataRow row = ds.Tables[0].Rows[i];
                //    DataRow rowgird = this.dsDetail.Tables[0].NewRow();
                //    string jzlsh = row["jzlsh"].ToString();
                //    string djh = row["djh"].ToString();
                //    SY.DZ.YBDetail detail = new SY.DZ.YBDetail();
                //    (code, result) = yb_interface_sy_zrNew.DetailDZ(jzlsh, djh, ref detail);
                //    if (code <= 0)
                //    {
                //        MessageBox.Show(result);
                //        return;
                //    }
                //    rowgird["门诊住院流水号"] = jzlsh;
                //    rowgird["发票号"] = djh;
                //    rowgird["his医疗总金额"] = Decimal.Parse(row["ylfze"].ToString());
                //    rowgird["his统筹支付金额"] = Decimal.Parse(row["tczfje"].ToString());
                //    rowgird["his现金支付金额"] = Decimal.Parse(row["xjzfje"].ToString());
                //    rowgird["医保医疗总金额"] = Decimal.Parse(string.IsNullOrEmpty(detail.medfee_sumamt) ? "0.00" : detail.medfee_sumamt);
                //    rowgird["医保统筹支付金额"] = Decimal.Parse(string.IsNullOrEmpty(detail.hifp_pay) ? "0.00" : detail.hifp_pay);
                //    rowgird["医保现金支付金额"] = Decimal.Parse(string.IsNullOrEmpty(detail.psn_cash_pay) ? "0.00" : detail.psn_cash_pay);
                //    this.dsDetail.Tables[0].Rows.Add(rowgird);
                //} 
                #endregion

                #region 使用分布式异步
                this.fbsSumGetRow(dsmx).Wait();

                //Func<DataSet, Task> AsyncAction = fbsSumGetRow;
                //AsyncAction.BeginInvoke(dsmx, (ar) =>
                //{
                //    AsyncAction.EndInvoke(ar);

                //    this.lblMemo.Text = "已成功核对完成！！";
                //    this.lblMemo.ForeColor = Color.Green;
                //}, null);
                #endregion
                this.dgvDetail.DataSource = this.dsDetail.Tables[0];
                for (int i = 0; i < dgvDetail.Rows.Count; i++)
                {
                    if (dgvDetail.Rows[i].Cells["收费员"].Value.ToString()!=CliUtils.fLoginUser)
                    {
                        this.dgvDetail.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                    }
                }

                this.lblMemo.Text = "已成功核对完成！！";
                this.lblMemo.ForeColor = Color.Green;
                #endregion
            }



            this.lblMemo.Text = "已成功核对完成！！";
            this.lblMemo.ForeColor = Color.Green;
        }

        /// <summary>
        /// 分布式算法进行获取行
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public Task fbsSumGetRow(DataSet ds)
        {
            int sumcount = ds.Tables[0].Rows.Count;
            int count = 10;
            int cishu = sumcount % count > 0 ? (sumcount / count) + 1 : sumcount / count;
            Task task = Task.Factory.StartNew(() =>
            {
                List<Task<int>> tasklist = new List<Task<int>>();
                for (int i = 0; i < cishu; i++)
                {
                    int start = i * count;
                    int end = (i + 1) * count > sumcount ? sumcount : (i + 1) * count;
                    Task<int> task1 = this.fbsTask(start: start, end: end, ds: ds);
                    tasklist.Add(task1);
                }
                Task.WaitAll(tasklist.ToArray());
            });
            return task;
        }

        /// <summary>
        /// 分布式任务进行
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public Task<int> fbsTask(int start, int end, DataSet ds)
        {
            Task<int> task = Task.Factory.StartNew(() =>
            {
                for (int i = start; i < end; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];
                    DataRow rowgird = this.dsDetail.Tables[0].NewRow();
                    string jzlsh = row["jzlsh"].ToString();
                    string djh = row["djh"].ToString();
                    SY.DZ.YBDetail detail = new SY.DZ.YBDetail();
                    //if (row["jbr"].ToString()!=CliUtils.fLoginUser)
                    //{
                    //    continue;
                    //}
                    this.lblMemo.Text = $"正在核对就诊流水号为{jzlsh}单据号为{djh}的患者明细！请稍后！！";
                    this.lblMemo.ForeColor = Color.Red;
                    UserId = UserId == "ALL" ? "" : UserId;
                    (int code, string result) = yb_interface_sy_zrNew.DetailDZ(jzlsh, djh, ref detail);
                    if (code <= 0)
                    {
                        MessageBox.Show(result);
                        break;
                    }
                    rowgird["收费员"] = row["jbr"].ToString();
                    rowgird["门诊住院流水号"] = jzlsh;
                    rowgird["发票号"] = djh;
                    rowgird["his医疗总金额"] = Decimal.Parse(row["ylfze"].ToString());
                    rowgird["his统筹支付金额"] = Decimal.Parse(row["tczfje"].ToString());
                    rowgird["his现金支付金额"] = Decimal.Parse(row["xjzfje"].ToString());
                    rowgird["医保医疗总金额"] = Decimal.Parse(string.IsNullOrEmpty(detail.medfee_sumamt) ? "0.00" : detail.medfee_sumamt);
                    rowgird["医保统筹支付金额"] = Decimal.Parse(string.IsNullOrEmpty(detail.hifp_pay) ? "0.00" : detail.hifp_pay);
                    rowgird["医保现金支付金额"] = Decimal.Parse(string.IsNullOrEmpty(detail.psn_cash_pay) ? "0.00" : detail.psn_cash_pay); 
                    this.dsDetail.Tables[0].Rows.Add(rowgird);
                }
                return 1;
            });
            return task;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)//对总账结果
            {
                int resCode = 1;
                string Res = string.Empty;
                decimal histotal = Convert.ToDecimal(dgvData.Rows[0].Cells[1].Value);//his医疗总金额
                decimal ybtotal = Convert.ToDecimal(dgvData.Rows[0].Cells[3].Value);//医保医疗总金额
                if (Math.Abs(histotal - ybtotal) > 1)//总金额
                {
                    resCode = -1;
                    Res += $"医疗总金额对账失败！his与医保金额的差值为：{Math.Abs(histotal - ybtotal)}\r\n";
                }
                decimal hispubcost = Convert.ToDecimal(dgvData.Rows[1].Cells[1].Value);//his统筹支付金额
                decimal ybpubcost = Convert.ToDecimal(dgvData.Rows[1].Cells[3].Value);//医保统筹支付金额
                if (Math.Abs(hispubcost - ybpubcost) > 1)//统筹金额
                {
                    resCode = -1;
                    Res += $"统筹支付金额对账失败！his与医保金额的差值为：{Math.Abs(hispubcost - ybpubcost)}\r\n";
                }
                decimal hisowncost = Convert.ToDecimal(dgvData.Rows[2].Cells[1].Value);//his现金支付金额
                decimal ybowncost = Convert.ToDecimal(dgvData.Rows[2].Cells[3].Value);//医保现金支付金额  
                if (Math.Abs(hisowncost - ybowncost) > 1)
                {
                    resCode = -1;
                    Res += $"现金支付金额对账失败！his与医保金额的差值为：{Math.Abs(hisowncost - ybowncost)}\r\n";
                }
                if (resCode == -1)
                {
                    this.dgvData.Rows[3].Cells[1].Style.ForeColor = Color.Red;
                }
                else
                {
                    Res = "总账对账成功！";
                    this.dgvData.Rows[3].Cells[1].Style.ForeColor = Color.Green;
                }
                this.dgvData.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                this.dgvData.Rows[3].Cells[1].ValueType = typeof(string);
                this.dgvData.Rows[3].Cells[1].Value = Res;
            }
            else if (tabControl1.SelectedIndex == 1)//对明细结果
            {
                this.dgvResult.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                this.dsResult.Tables[0].Rows.Clear();
                for (int i = 0; i < this.dgvDetail.Rows.Count; i++)
                {
                    
                    DataRow drdetail = this.dsResult.Tables[0].NewRow();
                    int code = 1; string res = string.Empty;
                    decimal histotal = Convert.ToDecimal(this.dgvDetail.Rows[i].Cells["his医疗总金额"].Value);
                    decimal hispubcost = Convert.ToDecimal(this.dgvDetail.Rows[i].Cells["his统筹支付金额"].Value);
                    decimal hisowncost = Convert.ToDecimal(this.dgvDetail.Rows[i].Cells["his现金支付金额"].Value);
                    decimal ybtotal = Convert.ToDecimal(this.dgvDetail.Rows[i].Cells["医保医疗总金额"].Value);
                    decimal ybpubcost = Convert.ToDecimal(this.dgvDetail.Rows[i].Cells["医保统筹支付金额"].Value);
                    decimal ybowncost = Convert.ToDecimal(this.dgvDetail.Rows[i].Cells["医保现金支付金额"].Value);
                    res = "对账成功！";
                    if (Math.Abs(histotal - ybtotal) > 1)
                    {
                        code = -1;
                        res += $"总额:{Math.Abs(histotal - ybtotal)}\r\n";
                    }
                    if (Math.Abs(hispubcost - ybpubcost) > 1)
                    {
                        code = -1;
                        res += $"统筹:{Math.Abs(hispubcost - ybpubcost)}\r\n";
                    }
                    if (Math.Abs(hisowncost - ybowncost) > 1)
                    {
                        code = -1;
                        res += $"现金:{Math.Abs(hisowncost - ybowncost)}\r\n";
                    }
                   
                    if (hisowncost+ybowncost==0)
                    {
                        code = 1;
                        res = "已退费！";
                    }
                    if (this.dgvDetail.Rows[i].Cells["收费员"].Value.ToString() != CliUtils.fLoginUser)
                    {
                        code = -1;
                        res = "不得对其他收费员的帐！";
                    }
                    if (code == -1)
                    {
                        drdetail["收费员"] = this.dgvDetail.Rows[i].Cells["收费员"].Value.ToString();
                        drdetail["门诊住院流水号"] = this.dgvDetail.Rows[i].Cells["门诊住院流水号"].Value.ToString();
                        drdetail["发票号"] = this.dgvDetail.Rows[i].Cells["发票号"].Value.ToString();
                        drdetail["his医疗总金额"] = this.dgvDetail.Rows[i].Cells["his医疗总金额"].Value.ToString();
                        drdetail["his统筹支付金额"] = this.dgvDetail.Rows[i].Cells["his统筹支付金额"].Value.ToString();
                        drdetail["his现金支付金额"] = this.dgvDetail.Rows[i].Cells["his现金支付金额"].Value.ToString();
                        drdetail["医保医疗总金额"] = this.dgvDetail.Rows[i].Cells["医保医疗总金额"].Value.ToString();
                        drdetail["医保统筹支付金额"] = this.dgvDetail.Rows[i].Cells["医保统筹支付金额"].Value.ToString();
                        drdetail["医保现金支付金额"] = this.dgvDetail.Rows[i].Cells["医保现金支付金额"].Value.ToString();
                        drdetail["对账结果"] = res;
                        this.dsResult.Tables[0].Rows.Add(drdetail);

                        this.dgvResult.Rows[i].DefaultCellStyle.ForeColor= Color.Red;
                    }
                    else if (code==1)
                    {
                        drdetail["收费员"] = this.dgvDetail.Rows[i].Cells["收费员"].Value.ToString();
                        drdetail["门诊住院流水号"] = this.dgvDetail.Rows[i].Cells["门诊住院流水号"].Value.ToString();
                        drdetail["发票号"] = this.dgvDetail.Rows[i].Cells["发票号"].Value.ToString();
                        drdetail["his医疗总金额"] = this.dgvDetail.Rows[i].Cells["his医疗总金额"].Value.ToString();
                        drdetail["his统筹支付金额"] = this.dgvDetail.Rows[i].Cells["his统筹支付金额"].Value.ToString();
                        drdetail["his现金支付金额"] = this.dgvDetail.Rows[i].Cells["his现金支付金额"].Value.ToString();
                        drdetail["医保医疗总金额"] = this.dgvDetail.Rows[i].Cells["医保医疗总金额"].Value.ToString();
                        drdetail["医保统筹支付金额"] = this.dgvDetail.Rows[i].Cells["医保统筹支付金额"].Value.ToString();
                        drdetail["医保现金支付金额"] = this.dgvDetail.Rows[i].Cells["医保现金支付金额"].Value.ToString();
                        drdetail["对账结果"] = res;
                        this.dsResult.Tables[0].Rows.Add(drdetail);

                        this.dgvResult.Rows[i].DefaultCellStyle.ForeColor = Color.Green;
                    }
                }
                if (this.dsResult.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("医保明细对账完成，没有任何异常数据！", "提示");
                }
                this.dgvResult.DataSource = this.dsResult.Tables[0];
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FrmZZ zz = new FrmZZ();
            //zz.Show();
            //FrmYBRegister r = new FrmYBRegister();
            //r.ShowDialog();
            Frmybxmdz dz = new Frmybxmdz();
            dz.ShowDialog();
        }
    }
}
