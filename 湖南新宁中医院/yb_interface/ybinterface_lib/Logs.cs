using Srvtools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
	public class Logs
	{
		public static void WriteLog(string str)
		{
			try
			{
				if (!Directory.Exists("YBLog"))
				{
					Directory.CreateDirectory("YBLog");
				}
				FileStream fileStream = new FileStream("YBLog\\YBLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.WriteLine(str);
				streamWriter.Close();
				fileStream.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("写医保日志异常|" + ex.ToString());
			}
		}

		public static void InsertYBLog(string jzlsh, string inParam, string outParam)
		{
			string text = string.Format("insert into ybrzwx(jzlsh, inParam, outParam, sysdate) values('{0}', '{1}', '{2}', '{3}')", jzlsh, inParam, outParam, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			object[] objParams = new object[1]
			{
			text
			};
			try
			{
				objParams = CliUtils.CallMethod("sybdj", "BatExecuteSql", objParams);
			}
			catch (Exception ex)
			{
				MessageBox.Show("写医保日志数据库异常|" + ex.ToString());
			}
		}
	}
}
