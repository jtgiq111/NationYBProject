using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class frm_dk : Form
    {
        public string sValue = string.Empty;
        public frm_dk()
        {
            InitializeComponent();
        }

        public frm_dk(string param)
        {
            sValue = param;
            InitializeComponent();
        }

        private void frm_dk_Load(object sender, EventArgs e)
        {
            string[] str = sValue.Split(';');
            DataTable dt = new DataTable();
            dt.Columns.Add("grbh", typeof(string));
            dt.Columns.Add("xm", typeof(string));
            dt.Columns.Add("xb",typeof(string));
            dt.Columns.Add("csrq",typeof(string));
            dt.Columns.Add("gmsfhm", typeof(string));
            dt.Columns.Add("xslb",typeof(string));
            dt.Columns.Add("dwmc", typeof(string));
            dt.Columns.Add("cbdmc", typeof(string));
            foreach (string s1 in str)
            {
                string[] s = s1.Split('|');
                DataRow dr = dt.NewRow();
                dr["grbh"] = s[0];
                dr["xm"] = s[1];
                dr["xb"] = s[2];
                dr["csrq"] = s[3];
                dr["gmsfhm"] = s[4];
                dr["xslb"] = s[5];
                dr["dwmc"] = s[6];
                dr["cbdmc"] = s[7];
                dt.Rows.Add(dr);
            }
            dgv_dk.DataSource = dt;

        }

        private void dgv_dk_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            sValue = dgv_dk.Rows[e.RowIndex].Cells["col_grbh"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_xm"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_xb"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_csrq"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_gmsfhm"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_xslb"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_dwmc"].Value.ToString();
            sValue += "|" + dgv_dk.Rows[e.RowIndex].Cells["col_cbdmc"].Value.ToString();
            this.Close();
        }


    }
}
