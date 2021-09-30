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
        private int page = 20;
        private int dqye = 1;
        private int sumpage = 10;
        public Frmzd(string hisxmmc,DataSet ds)
        {
            InitializeComponent();
            this.hisxmmc = hisxmmc.Replace("%","").Replace("[","").Replace("]","");
            this.dstotal = ds;
            sumpage = this.dstotal.Tables[0].Rows.Count % page > 0 ? ((this.dstotal.Tables[0].Rows.Count / page) + 1) : (this.dstotal.Tables[0].Rows.Count / page);
            init();
        }
        internal DataGridViewRow dr = null;


        internal DataSet dstotal = new DataSet();
        public void init()
        { 
            Fliter(hisxmmc);

            (from DataColumn dc in dstotal.Tables[0].Columns select dc.ColumnName).ToList().ForEach(c => { newdt.Columns.Add(c); });
            this.txtFilterword.Text = hisxmmc;
            this.dgvzdinfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvzdinfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvzdinfo.AllowUserToAddRows = false;
            this.dgvzdinfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect; 

        }
        private List<DataRow> drlist = new List<DataRow>();
        private DataTable newdt = new DataTable();
        public void Fliter(string text)
        {
            this.lblfy.Text = $"当前页{page}/{sumpage}页";
            newdt.Rows.Clear();

            drlist = (from DataRow dr in dstotal.Tables[0].Rows
                      where dr["国家项目编码"].ToString().Contains(text) || dr["国家项目名称"].ToString().Contains(text)|| dr["拼音码"].ToString().Contains(text) || dr["五笔码"].ToString().Contains(text)||"ALL"==text
                      select dr).Skip((dqye - 1)*page).Take(dqye*page).ToList();
            drlist.ForEach(row =>
            {
                newdt.Rows.Add(row);
            });
            //DataView dv = dstotal.Tables[0].DefaultView; 
            //string filterStr = string.Format(@"国家项目编码 like '%{0}%' or 国家项目名称 like '%{0}%' or 拼音码 like '%{0}%' or 五笔码 like '%{0}%' or 'ALL'='{0}' ", text);
            //dv.RowFilter = filterStr;
             
            this.dgvzdinfo.DataSource = newdt;
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

        private void button2_Click(object sender, EventArgs e)
        {
            page--;
            if (page<1)
            {
                MessageBox.Show("已经达到最底页");
                return;
            }
            Fliter(this.txtFilterword.Text.Trim());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            page++;
            if (page>sumpage)
            {
                MessageBox.Show("已经达到最顶页");
                return;
            }
            Fliter(this.txtFilterword.Text.Trim());
        }
    }
}
