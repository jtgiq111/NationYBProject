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
    public partial class frm_dkHF : Form
    {
        public string sValue = string.Empty;
        public frm_dkHF()
        {
            InitializeComponent();
        }

        private void frm_dk_Load(object sender, EventArgs e)
        {
            #region 参保人所属
            IList<Info> infoList = new List<Info>();
            Info info1 = new Info() { Id = "340199", Name = "合肥市本级" };
            Info info2 = new Info() { Id = "340121", Name = "长丰县" };
            Info info3 = new Info() { Id = "340122", Name = "肥东县" };
            Info info4 = new Info() { Id = "340123", Name = "肥西县" };
            Info info5 = new Info() { Id = "340181", Name = "巢湖市" };
            Info info6 = new Info() { Id = "340124", Name = "庐江县" };
            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            infoList.Add(info4);
            infoList.Add(info5);
            infoList.Add(info6);
            comboBox1.DataSource = infoList;
            comboBox1.ValueMember = "Id";
            comboBox1.DisplayMember = "Name";
            #endregion

            #region 险种
            IList<Info> infoList_xz = new List<Info>();
            Info info1_xz = new Info() { Id = "310", Name = "医疗保险" };
            Info info2_xz = new Info() { Id = "410", Name = "工伤保险" };
            Info info3_xz = new Info() { Id = "510", Name = "生育保险" };
            infoList_xz.Add(info1_xz);
            infoList_xz.Add(info2_xz);
            infoList_xz.Add(info3_xz);
            comboBox2.DataSource = infoList_xz;
            comboBox2.ValueMember = "Id";
            comboBox2.DisplayMember = "Name";

            #endregion

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string kh = txtICKH.Text.Trim();
            string cbss = comboBox1.SelectedValue.ToString();
            string xz = comboBox2.SelectedValue.ToString();
            sValue = "1|" + kh + "|" + cbss + "|" + xz;
            this.Close();
        }

        private void btnCANCEL_Click(object sender, EventArgs e)
        {
            sValue = "0";
            this.Close();
        }


    }
    public class Info
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }

}
