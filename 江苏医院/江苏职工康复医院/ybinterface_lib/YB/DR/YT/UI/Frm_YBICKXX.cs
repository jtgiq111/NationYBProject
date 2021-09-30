using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_YBICKXX : Form
    {
        private string ybkxx = "";

        public Frm_YBICKXX()
        {
            InitializeComponent();
        }

        public Frm_YBICKXX(string param)
        {
            InitializeComponent();
            ybkxx = param;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Frm_YBICKXX_Load(object sender, EventArgs e)
        {
            try
            {
                string[] sValue = ybkxx.Split('|');
                /*
                 * 1		个人编号	VARHCAR2(20)		
                    2		单位编号	VARHCAR2(16)		
                    3		身份证号	VARHCAR2(20)		
                    4		姓名	VARHCAR2(50)		
                    5		性别	VARHCAR2(3)		二级代码
                    6		民族	VARHCAR2(3)		二级代码
                    7		出生日期	VARHCAR2(8)		YYYYMMDD
                    8		社会保障卡卡号	VARHCAR2(20)		
                    9		医疗待遇类别	VARHCAR2(3)		二级代码
                    10		人员参保状态	VARHCAR2(3)		二级代码
                    11		异地人员标志	VARHCAR2(3)		二级代码
                    12		统筹区号	VARHCAR2(6)		
                    13		年度	VARHCAR2(4)		
                    14		在院状态	VARHCAR2(3)		
                    15		帐户余额	VARHCAR2(16)		2位小数
                    16		本年医疗费累计	VARHCAR2(16)		2位小数
                    17		本年帐户支出累计	VARHCAR2(16)		2位小数
                    18		本年统筹支出累计	VARHCAR2(16)		2位小数
                    19		本年救助金支出累计	VARHCAR2(16)		2位小数
                    20		本年公务员补助基金累计	VARHCAR2(16)		2位小数
                    21		本年城镇居民门诊统筹支付累计	VARHCAR2(16)		2位小数
                    22		进入统筹费用累计	VARHCAR2(16)		2位小数
                    23		进入救助金费用累计	VARHCAR2(16)		2位小数
                    24		起付标准累计	VARHCAR2(16)		2位小数
                    25		本年住院次数	VARHCAR2(3)		
                    26		单位名称	VARHCAR2(100)		
                    27		年龄	VARHCAR2(3)		
                    28		参保单位类型	VARHCAR2(3)		二级代码
                    29		经办机构编码	VARHCAR2(16)		二级代码
                    30	二类门慢限额支出	VARCHAR2(16)		【景德镇】专用
                    31	二类门慢限额剩余	VARCHAR2(16)		【景德镇】专用
                    32	医疗待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                    33	工伤待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                    34	生育待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                    35	慢性病审批有效时间不足30提示	VARCHAR2(200)		
                    36	保险公司	VARCHAR2(3)		二级代码
                    37	民政救助标志	VARCHAR2(3)		鹰潭用
                    38	居民优抚对象	VARCHAR2(3)		鹰潭用
                 */

                txtGRBH.Text = sValue[0];
                txtDWBH.Text = sValue[1];
                txtSFZH.Text = sValue[2];
                txtXM.Text = sValue[3];
                txtXB.Text = sValue[4];
                txtMZ.Text = sValue[5];
                txtCSRQ.Text = sValue[6];
                txtKH.Text = sValue[7];
                txtYLDYLB.Text = sValue[8];
                txtCBZT.Text = sValue[9];
                txtYDRYBZ.Text = sValue[10];
                txtSSQH.Text = sValue[11];
                txtND.Text = sValue[12];
                txtZYZT.Text = sValue[13];
                txtZFYE.Text = sValue[14];
                txtBNYLFLJ.Text = sValue[15];
                txtBNZHZCLJ.Text = sValue[16];
                txtBNTCZCLJ.Text = sValue[17];
                txtBNJZJZCLJ.Text = sValue[18];
                txtGWYBZJJLJ.Text = sValue[19];
                txtJMMZTCZFLJ.Text = sValue[20];
                txtJRTCFYLJ.Text = sValue[21];
                txtJRJZJFYLJ.Text = sValue[22];
                txtQFBZLJ.Text = sValue[23];
                txtZYCS.Text = sValue[24];
                txtDWMC.Text = sValue[25];
                txtSJNL.Text = sValue[26];
                txtDWLX.Text = sValue[27];
                //OP.Jbjgbm = sValue[28];
                //OP.Elmmxezc = sValue[29];
                //OP.Elmmxesy = sValue[30];
                //OP.Jjlx = sValue[31];
                //OP.Gsbxcbbz = sValue[32];
                //OP.Sybxcbbz = sValue[33];
                //OP.Mxbmzyy = sValue[34];
                if (sValue[10].Equals("0"))
                {
                    txtBXGS.Text = sValue[35];
                    txtMZJZBZ.Text = sValue[36];
                    txtDBDXBZ.Text = sValue[37]; //低保对象标志 ,居民优抚对象
                }
                WriteLog("住院读卡显示成功");
            }
            catch (Exception ex)
            {
                WriteLog("住院读卡显示失败" + ex.Message);
            }
           
        }


        #region 日志
        public static void WriteLog(string str)
        {
            if (!Directory.Exists("YBLog"))
            {
                Directory.CreateDirectory("YBLog");
            }
            FileStream stream = new FileStream("YBLog\\YBLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Close();
            stream.Close();
        }
        #endregion

    }
}
