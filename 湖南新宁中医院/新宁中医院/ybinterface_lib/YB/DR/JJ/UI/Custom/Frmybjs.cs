using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frmybjs : InfoForm
    {
        DataSet _hzdeatil = new DataSet();
        string _rylb = string.Empty; //人员类别
        string _tcdq = string.Empty; //统筹地区
        string _jsh = string.Empty;   //发票号
        bool _isYB = false;           //是否为医保的标志
        bool _isJS = false;           //是否结算
        decimal _mzToZy = 0;          //门诊转住院费用  默认值为0
        
        string _compip = string.Empty;
        string _z1area = string.Empty;

        zy02 _jssub = new zy02();

        public Frmybjs()
        {
            InitializeComponent();
        }
        private string GetServerDate()
        {
            object[] myDateTime = CliUtils.CallMethod("GLModule", "GetServerTime", new object[] { });
            return (string)myDateTime[3];    //yyyy-MM-dd  
            //return (string)myDateTime[4];   // hh:mm:ss   
            //return (string)myDateTime[5];     //yyyy-MM-dd HH:mm:ss
        }
        private string GetServerDateTime()
        {
            object[] myDateTime = CliUtils.CallMethod("GLModule", "GetServerTime", new object[] { });
            //return (string)myDateTime[3];    //yyyy-MM-dd  
            //return (string)myDateTime[4];   // hh:mm:ss   
            return (string)myDateTime[5];     //yyyy-MM-dd HH:mm:ss
        }
        /// <summary>
        /// 预结算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yj_but_Click(object sender, EventArgs e)
        {
            string tishi_info = string.Empty;  //提示信息
            string strchkSql = string.Empty;
            string mjsflag = "0";
            string getserDatetime = string.Empty;
            string sjsh = "";  //结算号
            string jsrq = string.Format(@"{0} 00:00:00",labjsrq.Text);
            DataSet dsss = new DataSet();
            if (string.IsNullOrWhiteSpace(txt_feeTotal.Text)) txt_feeTotal.Text = "0.00";
            if (decimal.Parse(txt_feeTotal.Text) <= 0)
            {
                MessageBox.Show("此患者没有可预结算的费用,请确认后重新预结算","系统提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (_isJS)
            {
                MessageBox.Show("此患者已进行了结算!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            yj_but.Enabled = false;
            getserDatetime = GetServerDateTime();//获取服务器日期及时间
            if (m_radioButton.Checked)
            {
                sjsh = "M" + DateTime.Parse(getserDatetime).ToString("yyMMddHHmmss") + txt_zyno.Text.PadLeft(8, '0');
                mjsflag = "1";
                if (string.IsNullOrWhiteSpace(labjsrq.Text))
                {
                    MessageBox.Show("阶段结算日期不能为空,请选择阶段结算日期!!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    yj_but.Enabled = true;
                    return;
                }
                if (!_jssub.Checkjsfy(txt_zyno.Tag.ToString(), txt_zyno.Text, txt_feeTotal.Text, true, dtperiod.Value.ToString("yyyy-MM-dd")))
                {
                    yj_but.Enabled = true;
                    return;//判断患者费用是否有变化
                }
            }
            else if (e_radioButton.Checked)
            {
                if (string.IsNullOrWhiteSpace(lab_z1ldat.Text))
                {
                    MessageBox.Show("出院结算日期不能为空,请护士拖出科!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    yj_but.Enabled = true;
                    return;
                }
                if (!_jssub.Checkjsfy(txt_zyno.Tag.ToString(), txt_zyno.Text, txt_feeTotal.Text, false, dtperiod.Value.ToString("yyyy-MM-dd")))
                {
                    yj_but.Enabled = true;
                    return;//判断患者费用是否有变化
                }
                #region 办理出院条件查询
                if (!_jssub.leaveHospitalCondition(txt_zyno.Tag.ToString(), txt_zyno.Text))
                {
                    yj_but.Enabled = true;
                    return;
                }
                #endregion
                #region 判断是否患者是否出院
                string str = string.Empty;
                str = string.Format(@"select h.z1zyno from zy01h h 
                                where h.z1comp='{0}' and h.z1zyno='{1}' and h.z1ghno='{2}' and left(h.z1endv,1)='0' 
                                and exists(select 1 from zy01d z1d where h.z1comp=z1d.z1comp and h.z1ghno=z1d.z1ghno 
                                and h.z1zyno=z1d.z1zyno and left(z1d.z1endv,1)='8')",
                                CliUtils.fSiteCode, txt_zyno.Text, txt_zyno.Tag);
                dsss = CliUtils.ExecuteSql("shs01", "cmd", str, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                if (!(dsss.Tables[0].Rows.Count > 0))
                {
                    MessageBox.Show("该患者还没有出科,请先出科后办理结算", "系统提示");
                    txt_zyno.Focus();
                    yj_but.Enabled = true;
                    return;
                }
                #endregion
                sjsh = "E" + DateTime.Parse(getserDatetime).ToString("yyMMddHHmmss") + txt_zyno.Text.PadLeft(8, '0');
                mjsflag = "0";
            }
            if (!_jssub.Checkyjjfy(txt_zyno.Tag.ToString(), txt_zyno.Text, txt_amnt1.Text))
            {
                yj_but.Enabled = true;
                return;//判断患预交金是否有变化
            }
            if (_isYB)
            {
                #region 医保计算
                string wkybbz = string.Empty;
                if (zhsybz_ck.Checked)  //是否账户支付
                {
                    wkybbz = "1";
                }
                else
                {
                    wkybbz = "0";
                }
                //#region 读卡操作
                //object[] objj = { };
                //string jycode_dk = "2201";
                //objj = yb_interface.ybs_interface(jycode_dk, objj);
                //if (objj[1].ToString().Trim() == "1")
                //{
                //    string[] splitstr = objj[2].ToString().Split(new char[] { '|' });
                //    if (splitstr[10].ToString().Trim() == "1")
                //    {
                //        if (mjsflag == "1")
                //        {
                //            MessageBox.Show("异地卡不能价段性预结算", "医保提示");
                            yj_but.Enabled = true;
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        //_isLocaljs = true;//是否是本地患者
                //    }
                //}
                //else
                //{
                //    if(string.IsNullOrWhiteSpace(objj[2].ToString()))
                //    {
                //        MessageBox.Show("读卡失败,请重新插入卡重试", "医保提示");
                //    }
                //    else
                //    {
                //        MessageBox.Show(objj[2].ToString().Trim(), "医保提示");
                //    }
                //    yj_but.Enabled = true;
                //    return;
                //}
                //#endregion
                // 就诊流水号（住院号),个人编号
                // 1 标识0
                // 2 调用状态
                // 3 返回调用错误信息
                string ylzfy = string.Empty;      //医疗费合计 
                string zbxfy = string.Empty;      //总报销金额
                string zhzf = string.Empty;       //本次账户支付 IC卡划帐 
                string tcjz = string.Empty;       //统筹记帐 
                string grxjzf = string.Empty;     //病人付现金 
                string grzhye = string.Empty;     //个人帐户余额 
                string zxjz = string.Empty;       //专项救助
                string dejjzf = string.Empty;     //大病基金支付
                object[] obj = null;
                //住院费用预结算
                //就诊流水号、出院原因代码、账户使用标志代码、中途结算标志代码,结算时间(格式：字段的日期格式)
                obj = new object[] { txt_zyno.Text, cmbcyyy.SelectedValue, wkybbz, mjsflag, jsrq};
                obj = yb_interface.ybs_interface("4400", obj);
                if (obj[1].ToString().Trim().Equals("1"))
                {
                    //MessageBox.Show(obj[2].ToString());
                    string[] outvalues = obj[2].ToString().Split('|');
                    ylzfy = outvalues[0].ToString();      //医疗总费用 
                    zbxfy = outvalues[1].ToString();      //总报销金额
                    tcjz = outvalues[2].ToString();      //本次基金支付
                    zhzf = outvalues[4].ToString();      //本次账户支付 

                    grxjzf = outvalues[5].ToString();     //个人现金支付
                    grzhye = outvalues[25].ToString();    //本次结算前帐户余额
                    zxjz = (decimal.Parse(zbxfy) - decimal.Parse(tcjz) - decimal.Parse(zhzf)).ToString();//专项救助

                    dejjzf = "0";
                    //MessageBox.Show("医疗总费用" + obj[15].ToString() + "\r\n"+
                    //    "本次账户支付 " + obj[16].ToString() + "\r\n"+
                    //    "本次基金支付" + obj[18].ToString() + "\r\n"+
                    //    "个人现金支付" + obj[17].ToString() + "\r\n" +
                    //    "个人帐户余额" + obj[3].ToString() + "\r\n" +
                    //    "专项救助" + obj[20].ToString() + "\r\n"+
                    //    "大病基金支出" + obj[19].ToString() + "\r\n"
                    //    );
                    if (Math.Abs(decimal.Parse(ylzfy) - decimal.Parse(txt_feeTotal.Text)) > 1)
                    {
                        if (DialogResult.Cancel == MessageBox.Show(string.Format("当前总费用:{0}与医保总费用:{1}大于1", txt_feeTotal.Text, ylzfy) + ",是否继续预结算", "提示", MessageBoxButtons.OKCancel))
                        {
                            yj_but.Enabled = false;
                            return;
                        }
                    }

                    MessageBox.Show("医保住院预结算成功", "系统提示");

                    yj_but.Tag = zhsybz_ck.Checked; //总可报销金额 = 医疗总费用 - 账户支付 - 专项救助 - 个人现金支付 - 大病基金支出 
                    decimal tcje = decimal.Parse(tcjz);
                    txt_jjzf.Text = tcjz;
                    txt_tczf.Text = tcje.ToString();    //统筹支付
                    txt_zhzf.Text = zhzf.ToString();    //本次账户支付 
                    txt_iczfye.Text = grzhye.ToString();//本次结算前帐户余额
                    txt_iczfhye.Text = (decimal.Parse(grzhye) - decimal.Parse(zhzf)).ToString();//账户支付后余额
                }
                else
                {
                    MessageBox.Show(string.Format("{0}", obj[2].ToString()), "医保系统提示");
                    yj_but.Enabled = true;
                    return;
                }
                #endregion
            }
            m_radioButton.Enabled = false;
            e_radioButton.Enabled = false;
            js_but.Enabled = true;
            repprint_but.Enabled = true;
            txt_zyno.ReadOnly = true;
            yj_but.Enabled = true;
            _jsh = sjsh;   //产生医保结算号
            }


        private void js_but_Click(object sender, EventArgs e)
        {

            string sql = string.Empty;
            string jsrq = string.Format(@"{0} 00:00:00", labjsrq.Text);
            js_but.Enabled = false;
            if (e_radioButton.Checked)
            {
                #region 办理出院条件查询
                if (!_jssub.leaveHospitalCondition(txt_zyno.Tag.ToString(), txt_zyno.Text))
                {
                    js_but.Enabled = true;
                    return;
                }
                #endregion

                #region 当前患者的担保信息
                if (!_jssub.getdbj(txt_zyno.Text))
                {
                    MessageBox.Show("该病人还有未取消的担保,请取消后再办理结算", "系统提示");
                    txt_zyno.Text = "";
                    txt_zyno.Focus();
                    js_but.Enabled = true;
                    return;
                }
                #endregion
            }
            if (_isYB)
            {
                #region 判断是否帐户支付
                if (!string.IsNullOrWhiteSpace(yj_but.Tag.ToString()))
                {
                    if (zhsybz_ck.Checked.ToString().Trim() != yj_but.Tag.ToString().Trim())
                    {
                        if (!zhsybz_ck.Checked)
                            MessageBox.Show("预结算使用了帐户支付,但结算未使用帐户支付,请重新预结算", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("预结算未使用帐户支付,但结算使用了帐户支付,请重新预结算", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        js_but.Enabled = true;
                        return;
                    }
                }
                #endregion
            }

            //if (!ybjssub.Checkjsfy(txt_zyno.Tag.ToString(), txt_zyno.Text, txt_feeTotal.Text, txt_amnt1.Text)) return;//判断患者费用是否有变化
            #region  读卡操作
            //if (isYB)
            //{
            //    //医保读卡操作
            //    object[] objj = { };
            //    string jycode_dk = "2201";
            //    objj = yb_interface.ybs_interface(jycode_dk, objj);
            //    //MessageBox.Show(objj[1].ToString());
            //    if (Convert.ToInt32(objj[1].ToString()) == 1)
            //    {
            //        string[] strP = objj[2].ToString().Split('|');
            //        if (lab_hznm.Text.ToString().Trim() != strP[3].ToString().Trim())
            //        {
            //            MessageBox.Show("患者姓名与读卡患者信息不一致，请确认卡信息", "系统提示");
            //            return;
            //        }
            //    }
            //} 
            #endregion

            js_but.Enabled = false;
            repprint_but.Visible = true;
            dtperiod.Enabled = false;
            if (_isYB)
            {
                string wkybbz = string.Empty;
                if (zhsybz_ck.Checked)    //是否支付支付标志
                {
                    wkybbz = "1";
                }
                else
                {
                    wkybbz = "0";
                }
                string mjsflag = "0";
                if (m_radioButton.Checked)//是否中途结算标志
                {
                    mjsflag = "1";
                }
                else
                {
                    mjsflag = "0";
                }
                object[] obj = null;
                //住院费用结算
                //费用结算(入参：就诊流水号,发票号,出院原因代码（门诊传9）,账户使用标志（0或1）,中途结算标志（0或1）,结算时间(格式：字段的日期格式))
                obj = new object[] { txt_zyno.Text, _jsh, cmbcyyy.SelectedValue, wkybbz, mjsflag, jsrq};
                obj = yb_interface.ybs_interface("4401", obj);
                if (Convert.ToInt32(obj[1].ToString()) == 1)
                {
                    MessageBox.Show("医保住院结算成功", "医保结算提示");
                }
                else
                {
                    MessageBox.Show(obj[2].ToString(), "医保结算提示");
                    js_but.Enabled = true;
                    return;
                }
                object []obb = new object[] { txt_zyno.Text,_jsh };
                obb=yb_interface.ybs_interface("5016",obj);
                #region 结算成功写结算号到zy01h及结算日期到zy01h
                //院别,住院号,住院流水号,结算号,结算日期,结算总费用
                object[] back_mess, param;
                sql = CliUtils.fSiteCode + "￥" + txt_zyno.Text + "￥" + txt_zyno.Tag.ToString() + "￥" + _jsh + "￥" + labjsrq.Text + "￥" + txt_feeTotal.Text;
                param = new object[] { sql };
                back_mess = CliUtils.CallMethod("szy02", "p_czy02_7", param);
                if (back_mess[1].ToString() != "sucess")
                {
                    MessageBox.Show(back_mess[1].ToString(),"系统提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    js_but.Enabled = true;
                    return;
                }
                
                #endregion
                //住院号，单据号，出院日期(格式：yyyy-MM-dd HH:mm:ss)
                obj = new object[] { txt_zyno.Text, _jsh, Convert.ToDateTime(jsrq).ToString("yyyy-MM-dd HH:mm:ss") };
                obj = yb_interface.ybs_interface("4403", obj);
            }
            _isJS = true; 
        }

        private void Quit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Clear_button_Click(object sender, EventArgs e)
        {
            txt_zyno.ReadOnly = false;
            lab_tes.Text = string.Empty;
            txt_zyno.Tag = string.Empty;
            txt_zyno.Text = string.Empty;

            lab_hznm.Text =string.Empty;
            lab_sex.Text =string.Empty;
            lab_age.Text = string.Empty;
            lab_z1date.Text =string.Empty;
            lab_ksnm.Text =string.Empty;
            lab_hzlx.Text = string.Empty;
            lab_ybno.Text = string.Empty;
            lab_bedno.Text = string.Empty;
            lab_bqxx.Text = string.Empty;

            txt_feeTotal.Text = "0.00";
            txt_jjzf.Text = "0.00";

            txt_amnt1.Text = "0.00";

            _hzdeatil.Clear();

            txt_mzzy.Text = "0.00";
            _mzToZy = 0;
            js_but.Enabled = false;
            repprint_but.Visible = false;
            _isYB = false;
            _isJS = false;
            lab_z1ldat.Text = string.Empty;
            dtperiod.Value = DateTime.Parse(GetServerDate());
            txt_zyno.Focus();
        }

        private void repprint_but_Click(object sender, EventArgs e)
        {
            string jsrq = string.Format(@"{0} 00:00:00", labjsrq.Text);
            if (!_isJS)
            {
                MessageBox.Show("此患者没有出院结算!", "系统提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //住院号，单据号，出院日期(格式：yyyy-MM-dd HH:mm:ss)
            object []obj = new object[] { txt_zyno.Text, _jsh,jsrq};
            obj = yb_interface.ybs_interface("4403", obj);

        }
        private bool jsHzIfno()
        {
            DataSet strDB = new DataSet();
            string strSql = string.Empty;
            txt_zyno.Text = txt_zyno.Text.PadLeft(8, '0');
            #region  获取患者基本信息
            strDB = _jssub.gethzInfo(txt_zyno.Text);
            #endregion
            if (strDB.Tables[0].Rows.Count > 0)
            {
                DataRow dr = strDB.Tables[0].Rows[0];
                txt_zyno.Text = dr["z1zyno"].ToString();
                txt_zyno.Tag = dr["z1ghno"].ToString();
                lab_hznm.Text = dr["z1hznm"].ToString();
                lab_sex.Text = dr["z1sexx"].ToString();
                lab_age.Text = dr["z1agex"].ToString() + dr["z1flag"].ToString();
                lab_bedno.Text = dr["z1bedn"].ToString();
                lab_z1date.Text = dr["z1date"].ToString();
                lab_z1ldat.Text = dr["z1ldat"].ToString();
                lab_ksnm.Text = dr["z1ksnm"].ToString();
                lab_bqxx.Text = dr["z1bqnm"].ToString();
                lab_hzlx.Text = dr["z1lynm"].ToString();
                labjz.Text = lab_hzlx.Text;
                lab_ybno.Text = dr["z1ybno"].ToString();
                lab_z1ldat.Text = dr["z1ldat"].ToString();
                _rylb = dr["rydl"].ToString();
                _tcdq = dr["tcdq"].ToString();
                //_z1area = dr["z1area"].ToString();
                //如果科室中含有产科时,则显示胎儿数，否则不显示
                lbl_tes.Visible = false;
                lab_tes.Visible = false;

                #region 查询当前患者是否是医保患者
                string strSqlYB = string.Format(@"select count(*)sl from ybmzzydjdr where jzlsh='{0}' and cxbz=1", txt_zyno.Text);
                DataSet dbds = CliUtils.ExecuteSql("szy02", "cmd", strSqlYB, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                if (Convert.ToInt32(dbds.Tables[0].Rows[0]["sl"].ToString()) > 0)
                {
                    _isYB = true;
                    lbl_mzzy.Visible = true;
                    txt_mzzy.Visible = true;
                    getMzToZy(CliUtils.fSiteCode, txt_zyno.Tag.ToString().Trim(), txt_zyno.Text.Trim());
                }
                else
                {
                    MessageBox.Show("此患者不是医保患者", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                #endregion
                zhsybz_ck.Enabled = true;
                gbjj.Visible = true;
                zhsybz_ck.Checked = false;
                gbzfInfo.Visible = true;

                #region 查询患者费用明细
                if (m_radioButton.Checked)
                {
                    _hzdeatil=gethzdetail(true, labjsrq.Text);
                }
                else
                {
                    _hzdeatil=gethzdetail(false, string.Empty);
                }
                fee_idgv.DataSource = _hzdeatil.Tables[0];
                if (_hzdeatil.Tables[0].Rows.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(_hzdeatil.Tables[0].Rows[0]["z3jshx"].ToString()))
                    {
                        if (_hzdeatil.Tables[0].Rows[0]["z3jshx"].ToString().Substring(0, 1).ToUpper() == "E")
                            _isJS = true;
                        else
                            _isJS = false;
                    }
                    else
                    {
                        _isJS = false;
                    }
                }
                else
                {
                    _isJS = false;
                }
                #endregion
                decimal yjk_qt = 0;
                #region 查询当前患者预交款
                strSql = string.Format(@"select  sum(case SUBSTRING(z3endv,1,1) when '4' then -isnull(z3amnt,0) else isnull(z3amnt,0) end) as z3amnt 
                                           from zy03d where isnull(z3jshx,'')='' and z3comp='{0}' and  z3zyno='{1}' and z3ghno='{2}' and left(z3kind,1)='1'", CliUtils.fSiteCode, txt_zyno.Text, txt_zyno.Tag.ToString());
                DataSet yjjDataset = CliUtils.ExecuteSql("szy02", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                yjk_qt = Convert.ToDecimal(yjjDataset.Tables[0].Rows[0]["z3amnt"].ToString() == "" ? "0" : yjjDataset.Tables[0].Rows[0]["z3amnt"].ToString());
                #endregion

                txt_zhzf.Text = "0.00";

                txt_iczfye.Text = "0.00";

                txt_iczfhye.Text = "0.00";
                decimal fyzehj = 0; //费用合计
                decimal jzzehj = 0; //记账合计

                for (int i = 0; i < fee_idgv.Rows.Count; i++)
                {
                    fyzehj += Convert.ToDecimal(fee_idgv.Rows[i].Cells["fyze"].Value.ToString());
                }

                //记账合计
                jzzehj = Convert.ToDecimal(txt_tczf.Text) + Convert.ToDecimal(txt_zhzf.Text);

                //显示费用总额
                txt_feeTotal.Text = Math.Round(fyzehj, 2, MidpointRounding.AwayFromZero).ToString("0.00");
                //显示记账合计
                txt_jjzf.Text = Math.Round(jzzehj, 2, MidpointRounding.AwayFromZero).ToString("0.00");


                txt_amnt1.Text = Math.Round(yjk_qt, 2, MidpointRounding.AwayFromZero).ToString("0.00");
            }
            else
            {
                MessageBox.Show("没有找到该患者信息,请确认后重新输入住院号结算", "系统提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                txt_zyno.Focus();
                return false;
            }
            yj_but.Enabled = true;
            yj_but.Focus();
            return true;
        }
        private void txt_zyno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                m_radioButton.Enabled = true;
                e_radioButton.Checked = true;
                e_radioButton.Enabled = true;
                Clear_button.Enabled = false;
                if (!jsHzIfno()) return;
                Clear_button.Enabled = true;
                repprint_but.Visible = false;
            }
        }

        /// <summary>
        /// 获取门诊转住院费用
        /// </summary>
        /// <param name="comp">院别</param>
        /// <param name="ghno">住院流水号</param>
        /// <param name="zyno">住院号</param>
        private void getMzToZy(string comp, string ghno, string zyno)
        {
            string strmzzy = string.Format(@"select isnull(SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end)),0) as z3amt2
                                            from zy03dz where z3comp='{0}' and z3ghno='{1}' and z3zyno='{2}' and isnull(z3jshx,'')='' ",
                   comp.Trim(), ghno.Trim(), zyno.Trim());
            DataSet dsmzzy = CliUtils.ExecuteSql("szy02", "cmd", strmzzy, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (dsmzzy.Tables[0].Rows.Count > 0)
            {
                _mzToZy = Convert.ToDecimal(dsmzzy.Tables[0].Rows[0]["z3amt2"].ToString());
                txt_mzzy.Text = _mzToZy.ToString("0.00");
            }
            else
            {
                _mzToZy = 0;
                txt_mzzy.Text = "0.00";
            }
            
        }
        private DataSet gethzdetail(bool isstage,string stageDate)
        {
            DataSet hzdeatil = new DataSet();
            string strSql = string.Empty,strcondition=string.Empty;
            if (isstage)
            {
                strcondition = string.Format(" and left(z3date,10)<='{0}'",stageDate);
            }
            else
            {
                strcondition = string.Empty;
            }
            strSql = string.Format(@"
                           with temp as(
                           select SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))as z3amt2, bzname b5sfnm,isnull(z3jshx,'')as z3jshx 
                           from zy03d 
                           left join bz05h on z3sfno=b5sfno
                           left join bztbd on bzcodn='A4' and bzkeyx=b5zyno
                           where substring(z3kind,1,1) in ('2','4') and isnull(z3jshx,'')='' and z3comp='{0}' and z3ghno='{1}'and z3zyno='{2}' {3}
                           group by isnull(z3jshx,''), bzname having SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))>0

                           union all

                           select SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))as z3amt2, bzname b5sfnm,isnull(z3jshx,'')as z3jshx 
                           from zy03dz 
                           left join bz05h on z3sfno=b5sfno
                           left join bztbd on bzcodn='A4' and bzkeyx=b5zyno
                           where substring(z3kind,1,1) in ('2','4') and isnull(z3jshx,'')='' and z3comp='{0}' and z3ghno='{1}'and z3zyno='{2}' {3}
                           group by isnull(z3jshx,''), bzname having SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))>0)

                           select SUM(z3amt2) z3amt2,b5sfnm,z3jshx from temp group by b5sfnm,z3jshx
                           ", CliUtils.fSiteCode, txt_zyno.Tag.ToString(), txt_zyno.Text, strcondition);
            hzdeatil = CliUtils.ExecuteSql("szy02", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            return hzdeatil;
        }
        
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Name == "m_radioButton" && rb.Checked == true)
            {
                yj_but.Text = "&Y阶段预结算";
                labjsrq.Text = dtperiod.Value.ToString("yyyy-MM-dd");
                Clear_button.Enabled = false;
                if (!jsHzIfno()) return;
                Clear_button.Enabled = true;
                repprint_but.Visible = false;
                dtperiod.Enabled = true;
                cmbcyyy.Focus();

            }
            else if (rb.Name == "e_radioButton" && rb.Checked == true)
            {
                yj_but.Text = "&Y出院预结算";
                labjsrq.Text = GetServerDate();
                Clear_button.Enabled = false;
                if (!jsHzIfno()) return;
                Clear_button.Enabled = true;
                repprint_but.Visible = false;
                dtperiod.Enabled = false;
                cmbcyyy.Focus();

            }
          
        }

        private void Frmybjs_Load(object sender, EventArgs e)
        {
            Dictionary<string, string[]> dict3 = new Dictionary<string, string[]>();
            dict3.Add("z1zyno", new string[] { "医生姓名", "80" });
            yj_but.Text = "&Y出院预结算";
            lab_hznm.Text = string.Empty;
            lab_sex.Text = string.Empty;
            lab_age.Text = string.Empty;
            lab_z1date.Text = string.Empty;
            lab_ksnm.Text = string.Empty;
            lab_hzlx.Text = string.Empty;
            lab_ybno.Text = string.Empty;
            lab_bedno.Text = string.Empty;
            lab_z1ldat.Text = string.Empty;
            lab_bqxx.Text = string.Empty;
            labjsrq.Text = GetServerDate();
            txt_zyno.Focus();
            _compip=CliUtils.fComputerIp.ToString();
        }

        private void zhsybz_ck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ck=sender as CheckBox;
            labzh.Visible = false;
            txt_zhzf.Visible = false;
            if (ck.Checked)
            {
                labzh.Visible = true;
                txt_zhzf.Visible = true;
            }
            
        }

        private void Frmybjs_Shown(object sender, EventArgs e)
        {
            string str = @"SELECT CODE,NAME FROM YBXMLBZD where LBMC='出院原因'";
            DataSet ds = CliUtils.ExecuteSql("szy02", "cmd", str, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            cmbcyyy.DataSource = ds.Tables[0];
            cmbcyyy.DisplayMember = "name";
            cmbcyyy.ValueMember = "code";
            cmbcyyy.SelectedIndex = 0;
        }

        private void dtperiod_ValueChanged(object sender, EventArgs e)
        {
            //阶段性结算日期要大于等于入院日期小于等于当前日期
            if (string.IsNullOrWhiteSpace(lab_z1date.Text)) return;
            if (DateTime.Parse(dtperiod.Value.ToString("yyyy-MM-dd"))<= DateTime.Parse(GetServerDate())
                && DateTime.Parse(dtperiod.Value.ToString("yyyy-MM-dd"))>= DateTime.Parse(lab_z1date.Text.Substring(0,10)))
            {
                
                labjsrq.Text = dtperiod.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                dtperiod.Value = DateTime.Parse(GetServerDate());
                labjsrq.Text = GetServerDate();
            }
            if (!jsHzIfno()) return;//重新计算患者总费用
        }

        private void button1_Click(object sender, EventArgs e)
        {

            object[] obj = new object[] { "00235279", "E20170801153000235279", Convert.ToDateTime(labjsrq.Text).ToString("yyyy-MM-dd HH:mm:ss") };
            obj = yb_interface.ybs_interface("4403", obj);
        }

    }
}