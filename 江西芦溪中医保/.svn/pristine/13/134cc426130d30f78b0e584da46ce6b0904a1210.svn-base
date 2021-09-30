using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using yb_interfaces.JX.UI;

namespace yb_interfaces
{
    public partial class Frm_YBdjFYscXN : InfoForm
    {
        string _zyno = string.Empty;
        public Frm_YBdjFYscXN()
        {
            InitializeComponent();
        }
        public Frm_YBdjFYscXN(string zyno)
        {
            InitializeComponent();
            _zyno = zyno;
        }

        private void Frm_YBdjFYsc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                //if (bl == true)
                //{
                //  tabControl1.SelectedIndex = 1;
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                //    bl = false;
                //}
                //else 
                //{
                //    tabControl1.SelectedTab = tabControl1.TabPages[0];
                //    bl = true;
                //}
            }
            if (e.KeyCode == Keys.F3)
            {
                //if (bl == true)
                //{
                //  tabControl1.SelectedIndex = 1;
                tabControl1.SelectedTab = tabControl1.TabPages[0];
                //    bl = false;
                //}
                //else 
                //{
                //    tabControl1.SelectedTab = tabControl1.TabPages[0];
                //    bl = true;
                //}
            }
        }
        Frm_ybbbrydrXN fb = new Frm_ybbbrydrXN();
        Frm_ybfyscXN fb2 = new Frm_ybfyscXN();
        private void Frm_YBdjFYsc_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_zyno))
            {
                fb.TopLevel = false;
                fb.FormBorderStyle = FormBorderStyle.None;
                fb.Dock = DockStyle.Fill;
                panel1.Controls.Add(fb);
                fb.Parent = panel1;
                fb.Show();

                fb2.TopLevel = false;
                fb2.Dock = DockStyle.Fill;
                fb2.FormBorderStyle = FormBorderStyle.None;

                panel2.Controls.Add(fb2);
                fb2.Parent = panel2;
                fb2.Show();
            }
            else 
            {
                fb.TopLevel = false;
                fb.FormBorderStyle = FormBorderStyle.None;
                fb.Dock = DockStyle.Fill;
                fb.txt_zyno.Text = _zyno;
                panel1.Controls.Add(fb);
                fb.Parent = panel1;
                fb.Show();

                fb2.TopLevel = false;
                fb2.Dock = DockStyle.Fill;
                fb2.txtZYNO.Text = _zyno;
                fb2.FormBorderStyle = FormBorderStyle.None;
                panel2.Controls.Add(fb2);
                fb2.Parent = panel2;
                fb2.Show();
            }
        }
    }
}
