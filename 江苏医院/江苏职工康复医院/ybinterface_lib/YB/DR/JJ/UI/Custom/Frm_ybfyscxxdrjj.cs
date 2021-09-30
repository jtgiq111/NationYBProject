using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybfyscxxdrjj : InfoForm
    {
        DataSet ds;

        public Frm_ybfyscxxdrjj(string jzlsh, string cfhs, string xm)
        {
            InitializeComponent();
            dgv_fymx.AutoGenerateColumns = false;
            string strSql = string.Format(@"select m.yyxmmc, y.ybxmmc, y.gg, m.dj, sum(m.sl) sl, sum(m.je) je, y.sflb, y.sfxmdj from 
                        (
                            --药品
                            select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd a 
                            where a.mcghno = '{0}' and a.mccfno in ({1})
                            union all
                            --检查/治疗
                            select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno , b.mbzlno cfh          
                            from mzb2d b 
                            where b.mbghno = '{0}' and b.mbzlno in ({1})
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d c 
                            where c.mbghno = '{0}' and c.mbzlno in ({1})
                            union all
                            --注射
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            left join bz09d on b9mbno = mdtwid 
                            left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdwayid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                            where mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdpprid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            where mdpqty > 0 and mdzsno in ({1})
                            union all
                            --处方划价
                            select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, b.ygysno ysdm, c.y1sflb, a.ygshno cfh
                            from yp17d a 
                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                            join yp01h c on c.y1ypno = a.ygypno
                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                        ) m 
                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh and y.scbz = 1
                        group by m.yyxmmc, y.ybxmmc, y.gg, m.dj, y.sflb, y.sfxmdj"
                                    , jzlsh, cfhs);//3秒
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                
            dgv_fymx.DataSource = this.ds.Tables[0];
            lbl_lsh.Text = jzlsh;
            lbl_xm.Text = xm;
            decimal zje = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                zje += Convert.ToDecimal(dr["je"]);
            }

            lbl_xmfyzj.Text = zje.ToString();
        }

        public Frm_ybfyscxxdrjj()
        {
            InitializeComponent();
            dgv_fymx.AutoGenerateColumns = false;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            DataView dv = ds.Tables[0].DefaultView;

            if (txt_byxm.Text.Trim() != "")
            {
                if (rbtn_bypym.Checked)
                {
                }
                else
                {
                    dv.RowFilter = "yyxmmc like '%" + txt_byxm.Text.Trim() + "%'";
                }
            }

            dgv_fymx.DataSource = dv;
            decimal zje = 0;

            foreach (DataRow dr in dv)
            {
                zje += Convert.ToDecimal(dr["je"]);
            }

            lbl_xmfyzj.Text = zje.ToString();
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

        private void Frm_ybfyscxxdrjj_Load(object sender, EventArgs e)
        {
            btn_close.Focus();
        }
    }
}
