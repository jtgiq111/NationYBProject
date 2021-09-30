using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ybinterface_lib
{
    public partial class frmYBChoose : InfoForm
    {
        public string lmc = string.Empty;
        public string falg = "1"; //0-只选择医保文件 1－选择并初始化医院文件
        public frmYBChoose()
        {
            InitializeComponent();
        }

        public frmYBChoose(string param)
        {
            InitializeComponent();
            falg = param;
        }

        private void frmYBChoose_Load(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select a.CSDM,b.CSMC,a.DLLMC,a.LMC from YBPZZD a
                                            left join YBCSZD b on a.CSDM=b.CSDM where a.QYBZ=1");
            DataSet dsCS = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvCS.SetDataSource(dsCS.Copy());
            #region 设置AutoListView
            alvCS.SetColumnParam.AddQueryColumn("CSDM");
            alvCS.SetColumnParam.AddQueryColumn("CSMC");
            alvCS.SetColumnParam.SetValueColumn("CSDM");
            alvCS.SetColumnParam.SetTextColumn("CSMC");
            alvCS.SetColumnParam.AddViewColumn("CSDM", "编码", 80);
            alvCS.SetColumnParam.AddViewColumn("CSMC", "名称", 200);
            txtYBCS.Text = dsCS.Tables[0].Rows[0]["CSMC"].ToString();
            txtYBCS.Tag = dsCS.Tables[0].Rows[0]["CSDM"].ToString();
            lmc = dsCS.Tables[0].Rows[0]["LMC"].ToString();
            #endregion
        }

        private void alvCS_AfterConfirm(object sender, EventArgs e)
        {
            lmc = alvCS.currentDataRow["LMC"].ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
                object[] objParam = null;
                string dllmc = "ybinterface_lib.dll";
                string ffmc = "YBINIT";
                string path = AppDomain.CurrentDomain.BaseDirectory + "Solution1\\" + dllmc;
                Assembly asm = Assembly.LoadFile(path);
                Type type = asm.GetType("ybinterface_lib." + lmc);
                MethodInfo method = type.GetMethod(ffmc);
                object obj = method.Invoke(null, new object[] { objParam });
                objParam = (object[])obj;

                if (objParam[1].ToString().Equals("1"))
                {
                    CliUtils.fLoginYbNo = objParam[2].ToString() + "|" + lmc;
                    this.Close();
                }
                else
                    MessageBox.Show("医保初始化失败"+objParam[2].ToString());
        }

        private void btnCANCEL_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
