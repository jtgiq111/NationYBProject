using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_auto.Forms
{
    public partial class Frmfeedetail : Form
    {
        public Frmfeedetail()
        {
            InitializeComponent();
        }

        public Frmfeedetail(DataSet ds)
        {
            InitializeComponent();
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ReadOnly = true; 
            this.dataGridView1.DataSource = ds.Tables[0];
        }
    }
}
