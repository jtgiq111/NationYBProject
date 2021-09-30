using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using System.Reflection;

namespace ybinterface_lib
{
    public partial class frmTest : InfoForm
    {
        public frmTest()
        {
            InitializeComponent();
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select YWDM,YWMC from YBYWDMZD where QYBZ=1 order by YWDM");
            DataSet dsYW = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvYW.SetDataSource(dsYW.Copy());
            alvYW.SetColumnParam.AddQueryColumn("YWMC");
            alvYW.SetColumnParam.AddQueryColumn("YWDM");
            alvYW.SetColumnParam.SetValueColumn("YWDM");
            alvYW.SetColumnParam.SetTextColumn("YWMC");
            alvYW.SetColumnParam.AddViewColumn("YWDM", "编码", 80);
            alvYW.SetColumnParam.AddViewColumn("YWMC", "名称", 200);
        }

        private void btnTEST_Click(object sender, EventArgs e)
        {
            try
            {
                object[] objParam = null;
                string YWDM = txtYW.Tag.ToString();
                string inParam = txtINPARAM.Text.Trim();
                if (string.IsNullOrEmpty(YWDM))
                {
                    MessageBox.Show("医保业务代码不能为空");
                    return;
                }

                //入参
                if (!string.IsNullOrEmpty(inParam))
                {
                    string[] Param = inParam.Split('$');
                    objParam = Param;
                }
                else
                {
                    string[] Param = { };
                    objParam = Param;

                }

                // 初始化时，直接映射接口实体
                if (YWDM.Equals("9000"))
                {
                    //ybinterface_lib.frmYBChoose ybchoose = new frmYBChoose();
                    //ybchoose.ShowDialog();
                    string dllmc = "ybinterface_lib.dll";
                    string lmc = string.Empty;
                    string ffmc = "YBINIT";
                    string strSql = string.Format(@"select * from ybpzzd where qybz=1");
                    DataSet dsPZ = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (dsPZ.Tables[0].Rows.Count == 0)
                    {
                        txtOUTPARAM.Text = "初始化失败,未找到启用配置";
                        return;
                    }
                    else if (dsPZ.Tables[0].Rows.Count > 1)
                    {

                        ybinterface_lib.frmYBChoose ybchoose = new frmYBChoose("0");
                        ybchoose.ShowDialog();
                        lmc = ybchoose.lmc;
                    }
                    else
                    {
                        lmc = dsPZ.Tables[0].Rows[0]["lmc"].ToString();
                    }
                    #region 调用接口方法
                    string path = AppDomain.CurrentDomain.BaseDirectory + "Solution1\\" + dllmc;
                    Assembly asm = Assembly.LoadFile(path);
                    Type type = asm.GetType("ybinterface_lib." + lmc);
                    MethodInfo method = type.GetMethod(ffmc);
                    object obj = method.Invoke(null, new object[] { objParam });
                    objParam = (object[])obj;

                    //if (objParam[1].ToString().Equals("1"))
                    CliUtils.fLoginYbNo = objParam[2].ToString() + "|" + lmc;
                    #endregion
                }
                else
                {
                    //其他医保业务操作
                    objParam = yb_interface.ybs_interface(YWDM, objParam);
                }
                
                txtOUTPARAM.Text = objParam[2].ToString();
            }
            catch (Exception ex)
            {
                txtOUTPARAM.Text = "系统异常|" + ex.Message;
            }
        }
    }
}
