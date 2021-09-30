using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces
{
    public partial class Frmzd : Form
    {
        public Frmzd()
        {
            InitializeComponent();
        }
        public string hisxmmc { get; set; }
        public Frmzd(string hisxmmc,DataSet ds)
        {
            InitializeComponent();
            this.hisxmmc = hisxmmc.Replace("%","").Replace("[","").Replace("]","");
            this.dstotal = ds;
            init();
        }
        internal DataGridViewRow dr = null;


        internal DataSet dstotal = new DataSet();
        public void init()
        {
//            string Sql = @"
//select dm as 国家编码,dmmc as 国家项目名称,pym as 拼音码, wbm as 五笔码 from ybmrdr ";
//            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Sql.ToString(), CliUtils.fLoginDB, true, CliUtils.fCurrentProject); 
//            this.dstotal = ds;
            Fliter(hisxmmc);
            this.txtFilterword.Text = hisxmmc;
            this.dgvzdinfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvzdinfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvzdinfo.AllowUserToAddRows = false;
            this.dgvzdinfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect; 

        }

        public void Fliter(string text)
        {
            DataView dv = dstotal.Tables[0].DefaultView;
            string filterStr = string.Format(@"国家项目编码 like '%{0}%' or 国家项目名称 like '%{0}%' or 拼音码 like '%{0}%' or 五笔码 like '%{0}%' or 'ALL'='{0}' ", text);
            dv.RowFilter = filterStr;
            DataTable dtres = dv.ToTable();
            this.dgvzdinfo.DataSource = dtres;
        }

        private void dgvzdinfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dr = this.dgvzdinfo.Rows[e.RowIndex];
            this.Close();
        }

        private void txtFilterword_TextChanged(object sender, EventArgs e)
        {
            string word = this.txtFilterword.Text.Trim();
            Fliter(word);
        }
    }
}
