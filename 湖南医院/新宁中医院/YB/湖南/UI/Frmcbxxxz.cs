using Srvtools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace yb_interfaces
{
    public partial class Frmcbxxxz : InfoForm
    {
        DataSet dsCbxx = null;
        public string RYCBXX = string.Empty; 

        public Frmcbxxxz()
        {
            InitializeComponent();
        }

        public Frmcbxxxz(string sfzh)
        {
            InitializeComponent();
            RYCBXX = string.Empty;
            string strSql = string.Format(@" select balc, b.bzname insutype, insutype insutypecode, psn_type, psn_insu_stas, psn_insu_date, paus_insu_date, c.bzname gwybz,cvlserv_flag, insuplc_admdvs, 
                emp_name, grbh, sfzh, xm from ybrycbxx a left join bztbd b on a.insutype=b.bzkeyx and b.bzcodn='insutype' 
                left join bztbd c on a.cvlserv_flag=b.bzkeyx and c.bzcodn='cvlserv_flag' 
                where sfzh='{0}' ", sfzh);
            dsCbxx = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            gridView.AutoGenerateColumns = false;
            gridView.DataSource = dsCbxx.Tables[0];
            string grbh = dsCbxx.Tables[0].Rows[0]["grbh"].ToString();

            strSql = string.Format(@" select MMBZBM,MMBZMC,ksrq bzksrq,jsrq bzjsrq from ybmxbdj where bxh='{0}' ", grbh);
            DataSet dsBzxx = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvbzxx.AutoGenerateColumns = false;
            dgvbzxx.DataSource = dsBzxx.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void btncz_Click(object sender, EventArgs e)
        {

        }

        private void btnxz2_Click(object sender, EventArgs e)
        {
            
        }

        private void gridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int aRow = gridView.CurrentRow.Index;
            string balc = gridView.Rows[aRow].Cells["balc"].Value.ToString();
            string insutype = gridView.Rows[aRow].Cells["insutypecode"].Value.ToString();
            string psn_type = gridView.Rows[aRow].Cells["psn_type"].Value.ToString();
            string psn_insu_stas = gridView.Rows[aRow].Cells["psn_insu_stas"].Value.ToString();
            string psn_insu_date = gridView.Rows[aRow].Cells["psn_insu_date"].Value.ToString();
            string paus_insu_date = gridView.Rows[aRow].Cells["paus_insu_date"].Value.ToString();
            string cvlserv_flag = gridView.Rows[aRow].Cells["cvlserv_flag"].Value.ToString();
            string insuplc_admdvs = gridView.Rows[aRow].Cells["insuplc_admdvs"].Value.ToString();
            string emp_name = gridView.Rows[aRow].Cells["emp_name"].Value.ToString();
            RYCBXX = balc + "|" + insutype + "|" + psn_type + "|" + psn_insu_stas + "|" + psn_insu_date + "|" + paus_insu_date + "|" + cvlserv_flag + "|" + insuplc_admdvs + "|" + emp_name + "|" ;
            this.Close();
        }

        private void infoDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
