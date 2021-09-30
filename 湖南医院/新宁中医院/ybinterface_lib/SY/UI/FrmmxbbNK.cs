using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib.SY.UI
{
    public partial class FrmmxbbNK : InfoForm
    {
        public FrmmxbbNK()
        {
            InitializeComponent();
            this.ControlBox = false;
        }
        private const int CPNOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CPNOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void Frmmxbbz_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing) 
            //{
            //    MessageBox.Show("请选择病种");
            //    e.Cancel = true;
            
            //}
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
             yb_interface_sy_zrNew1.frmbzbm = DataGridView.CurrentRow.Cells["bzbm"].Value.ToString();
            yb_interface_sy_zrNew1.frmbzmc = DataGridView.CurrentRow.Cells["bzmc"].Value.ToString();
             this.Close();
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    keyCode_Enter_Event();
                    break;
                case Keys.Up:
                    keyCode_Up_Event();
                    break;
                case Keys.Down:
                    keyCode_Down_Event();
                    break;
            }
        }
        /// <summary>
        /// Up快捷键事件
        /// </summary>
        private void keyCode_Up_Event()
        {
            if (DataGridView.Rows.Count != 0)    //当DataGridView为空的时候不执行任何操作
            {
                int iRowIndex = DataGridView.CurrentRow.Index;
                if (iRowIndex != 0) //当DataGridView当前行为第一行时不执行任何操作
                {
                    DataGridView.Rows[iRowIndex].Selected = true;
                    DataGridView.CurrentCell = DataGridView.Rows[iRowIndex].Cells[0];
                }
                //LoadTempBillMX();
            }

        }

        /// <summary>
        /// Enter快捷键事件
        /// </summary>
        private void keyCode_Enter_Event()
        {
            yb_interface_sy_zrNew1.frmbzbm = DataGridView.CurrentRow.Cells["bzbm"].Value.ToString();
            yb_interface_sy_zrNew1.frmbzmc = DataGridView.CurrentRow.Cells["bzmc"].Value.ToString();
            this.Close();

        }

        /// <summary>
        /// Down快捷键事件
        /// </summary>
        private void keyCode_Down_Event()
        {
            if (DataGridView.Rows.Count != 0)    //当DataGridView为空的时候不执行任何操作
            {
                int iRowIndex = DataGridView.CurrentRow.Index;
                if (iRowIndex != (DataGridView.Rows.Count - 1))  //当DataGridView当前行为最后一行时不执行任何操作
                {
                    DataGridView.Rows[iRowIndex].Selected = true;
                    DataGridView.CurrentCell = DataGridView.Rows[iRowIndex].Cells[0];
                }
                //LoadTempBillMX(); 
            }
        }
    }
}
