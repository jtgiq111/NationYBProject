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
    public partial class FrmTest1 : Form
    {
        public FrmTest1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] str = textBox1.Text.Trim().Split('|');

            textBox2.Text="总拆分:"+str.Length.ToString();
            int index = 0;
            foreach (string s in str)
            {
                textBox2.Text += s[index] + "\r\n";
                index++;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int index=textBox1.Text.IndexOf('-');
            textBox2.Text = textBox1.Text.Substring(0, index);
        }
    }
}
