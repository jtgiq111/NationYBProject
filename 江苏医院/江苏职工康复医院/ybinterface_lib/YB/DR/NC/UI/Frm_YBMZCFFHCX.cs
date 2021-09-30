using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using System.IO;

namespace ybinterface_lib
{
    public partial class Frm_YBMZCFFHCX : Form
    {
        string jzlsh = string.Empty;
        public Frm_YBMZCFFHCX()
        {
            InitializeComponent();
        }

        public Frm_YBMZCFFHCX(string param)
        {
            InitializeComponent();
            jzlsh = param;
        }

        private void Frm_YBMZCFFHCX_Load(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select
                                            b.yyxmmc as yysfxmmc,b.ybxmmc as sfxmzxmc,c.sfxmdj,b.je,b.zfje,
                                            case when b.sfxmdj='1' then '甲类'
                                            when  b.sfxmdj='2' then '乙类'
                                            when  b.sfxmdj='3' then '自费'
                                            when b.sfxmdj='4' then '丙类'
                                            else '自费' end as sfxmdj,
                                            case when zfbz='1' then '是'
                                            else '否' end zfbz,b.bz
                                            from ybcfmxscfhdr b 
                                            left join ybhisdzdr  c on b.yyxmdm=c.hisxmbh
                                            inner join ybfyyjsdr d on b.jylsh=d.cfmxjylsh and b.ybjzlsh=b.ybjzlsh
                                            where b.ybjzlsh='{0}' and b.cxbz=1 and b.yybxmbh!='60011020000100000000'", jzlsh);
            WriteLog(strSql);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        public static void WriteLog(string str)
        {
            if (!Directory.Exists("YBLog"))
            {
                Directory.CreateDirectory("YBLog");
            }
            FileStream stream = new FileStream("YBLog\\YBLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Close();
            stream.Close();
        }

    }
}
