using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Srvtools;

namespace ybinterface_lib
{
    public partial class Frm_Setting : InfoForm
    {
        public int[] s = { 0, 0 };         //用来记录from是否打开过
        public Frm_Setting()
        {
            InitializeComponent();
        }

        private void Frm_Setting_Load(object sender, EventArgs e)
        {
            Init();
        }

        internal void Init()
        {

            //TabPage tP = new TabPage();
            //tP.Tag = "ybinterface_lib.Frm_YBPZZD";
            //tabControl1.Controls.Add(tP);
            ////string formClass = "ybinterface_lib.Frm_YBPZZD";
            //GenerateForm(tP.Tag.ToString(), tabControl1);  
            GenerateForm();
        }


        public void GenerateForm()
        {
            string strSql = string.Format(@"select * from ybctpz where ctlb='01' and qybz=1 order by ctseq ");
            DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("未设置配置窗体");
                return;
            }

            int index = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TabPage tP = new TabPage();
                tP.Tag =dr["ctdm"];
                tabControl1.Controls.Add(tP);

                Form fm = (Form)Assembly.GetExecutingAssembly().CreateInstance(tP.Tag.ToString());

                //设置窗体没有边框 加入到选项卡中  
                fm.FormBorderStyle = FormBorderStyle.None;
                fm.TopLevel = false;
                fm.Parent = tP;
                fm.ControlBox = false;
                fm.Dock = DockStyle.Fill;
                fm.Show();

                s[this.tabControl1.SelectedIndex] = 1;
                this.tabControl1.TabPages[index].Text = fm.Text;
                index++;
            }
        }





        public void GenerateForm(string form, object sender)
        {
            // 反射生成窗体  
            Form fm = (Form)Assembly.GetExecutingAssembly().CreateInstance(form);

            //设置窗体没有边框 加入到选项卡中  
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.TopLevel = false;
            fm.Parent = ((TabControl)sender).SelectedTab;
            fm.ControlBox = false;
            fm.Dock = DockStyle.Fill;
            fm.Show();
           

            s[((TabControl)sender).SelectedIndex] = 1;
            //((TabControl)sender).TabPages[0].Text = fm.Text;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }  
    }
}
