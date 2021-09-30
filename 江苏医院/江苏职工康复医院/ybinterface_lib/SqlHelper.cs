using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Srvtools;

namespace ybinterface_lib
{
    public class SqlHelper : IDisposable
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        public string ConStr = null; //
        private bool AutoClose = true;
        IWork iWork = new Work();

        //private int Count = 0;

        public SqlHelper()
        {
            //string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
            ////List<Item> lItem = iWork.getXmlConfig(xmlPath + "ybConfig.xml");
            //Item lItem = iWork.getXmlConfig(xmlPath + "ybconfig.xml");
            //ConStr = lItem.SRCCON;
            //string key = "Gocent";
            //string test1 = "Data Source=192.168.1.246;Initial Catalog=DBGocent;User ID=sa;Password=ts@123";
            //string cryptstr = DEncrypt.Encrypt(test1, key);
            ConStr = "Data Source=192.168.1.246;Initial Catalog=DBGocent;User ID=sa;Password=ts@123";
        }
        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="conStr"></param>
        public void SetConstr(string conStr)
        {
            ConStr = conStr;
        }

        private SqlConnection _Conn = null;

        private SqlConnection Conn
        {
            get
            {
                if (_Conn == null)
                {
                    _Conn = new SqlConnection();
                    _Conn.ConnectionString = ConStr;
                }

                return _Conn;
            }
        }

        private void Open()
        {
            if (Conn.State != ConnectionState.Open)
            {
                Conn.Open();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="comText">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public bool ExecuteNonQuery(string comText, ref string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                SqlCommand cmd = new SqlCommand(comText, Conn);
                Open();
                cmd.ExecuteNonQuery();
                //Count = 0;
                return true;
            }
            catch (Exception ee)
            {
                errorMsg = ee.Message;
                return false;
                //throw new Exception(ee.Message);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }
            }

        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public bool ExecuteSqlTran(IList<string> comTextList, ref string errorMsg)
        {
            errorMsg = string.Empty;
            int count = 0;

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Conn;
                Open();

                for (int i = 0; i < comTextList.Count; i++)
                {
                    count = i;

                    if (comTextList[i].Length > 1)
                    {
                        cmd.CommandText = comTextList[i];
                        cmd.ExecuteNonQuery();
                    }
                }

                //Count = 0;
                return true;
            }
            catch (Exception ee)
            {
                errorMsg = ee.Message + "\n " + comTextList[count];
                return false;
                //throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }
            }
            //count = 0;

        }

        public object GetSingle(string comText, ref string errorMsg)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(comText, Conn);
                Open();
                object obj = cmd.ExecuteScalar();
                //Count = 0;

                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
            }
            catch (Exception e)
            {
                Close();
                errorMsg = comText + "\n" + e.Message;

                //if (Count < 1 && (e.Message.IndexOf("open") != -1 || e.Message.IndexOf("Open") != -1))
                //{
                //    Count++;
                //    return GetSingle(comText, ref errorMsg);
                //}

                throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }
            }

            //Count = 0;
            //return null;
        }

        /// <summary>
        /// 执行sql，并返回DataTable
        /// </summary>
        /// <param name="comText"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string comText, ref string errorMsg)
        {
            errorMsg = string.Empty;
            DataTable dt = new DataTable("DataRs");

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(comText, Conn);
                da.SelectCommand.CommandTimeout = 1000;
                da.Fill(dt);
                return dt;
            }
            catch (Exception ee)
            {
                errorMsg = ee.Message + "\n " + comText;

                //if (Count < 1 && (ee.Message.IndexOf("open") != -1 || ee.Message.IndexOf("Open") != -1))
                //{
                //    Count++;
                //    return ExecuteDataTable(comText, ref errorMsg);
                //}

                throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }

            }

            //Count = 0;

        }

        /// <summary>
        /// ylm add返回dataset
        /// </summary>
        /// <param name="comText">命令文本</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string comText, ref string errorMsg)
        {
            errorMsg = string.Empty;
            DataSet ds = new DataSet("DataDs");

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(comText, Conn);
                da.SelectCommand.CommandTimeout = 1000;
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                errorMsg = ee.Message + "\n " + comText;

                //if (Count < 1 && (ee.Message.IndexOf("open") != -1 || ee.Message.IndexOf("Open") != -1))
                //{
                //    Count++;
                //    return ExecuteDataSet(comText, ref errorMsg);
                //}

                throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }

            }

            //Count = 0;

        }

        /// <summary>
        /// 执行sql。返回DataSet
        /// </summary>
        /// <param name="comText"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string comText)
        {
            string errorMsg = string.Empty;
            return ExecuteDataSet(comText, ref errorMsg);
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="funNme"></param>
        /// <param name="errorMsg"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public bool ExecuteFunNonExecuteDataSet(string funNme, ref string errorMsg, params IDbDataParameter[] pars)
        {
            errorMsg = string.Empty;

            try
            {
                SqlCommand com = new SqlCommand();
                com.CommandType = CommandType.StoredProcedure;
                com.Connection = Conn;
                com.CommandText = funNme;

                foreach (IDbDataParameter par in pars)
                {
                    com.Parameters.Add(par);
                }

                com.CommandTimeout = 1000;
                Open();
                com.ExecuteNonQuery();
                //Count = 0;
                return true;
            }
            catch (Exception ee)
            {
                errorMsg = ee.ToString();

                //if (Count < 1 && (ee.Message.IndexOf("open") != -1 || ee.Message.IndexOf("Open") != -1))
                //{
                //    Count++;
                //    return ExecuteFunNonExecuteDataSet(funNme, ref errorMsg, pars);
                //}

                throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }

            }

            //Count = 0;
            //return true;
        }

        /// <summary>
        /// 插入记录返回自增长列
        /// </summary>
        /// <param name="comText"></param>
        /// <param name="columnName">为空返回自增长的列</param>
        /// <returns></returns>
        public Int64 InsertReturnID(string comText, ref string errorMsg, string columnName = "ID")
        {
            Int64 id = 0;
            errorMsg = string.Empty;
            comText += " select SCOPE_IDENTITY()";

            try
            {
                SqlCommand cmd = new SqlCommand(comText, Conn);
                Open();
                object obj = cmd.ExecuteScalar();
                //Count = 0;

                if (obj != null)
                {
                    id = Int64.Parse(obj.ToString());
                }

                return id;
            }
            catch (Exception ee)
            {
                errorMsg = ee.Message + "\n " + comText;

                //if (Count < 1 && (ee.Message.IndexOf("open") != -1 || ee.Message.IndexOf("Open") != -1))
                //{
                //    Count++;
                //    return InsertReturnID(comText, ref errorMsg, columnName);
                //}

                throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }

            }

            //Count = 0;
            //return id;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_Conn != null)
            {
                if (_Conn.State != ConnectionState.Closed)
                {
                    _Conn.Close();
                }
            }

            _Conn = null;
        }

        #endregion

        #region IDbHelper Members

        public bool ExecuteParaNonExecuteDataSet(string comText, ref string errorMsg, params IDbDataParameter[] pars)
        {
            errorMsg = string.Empty;

            try
            {
                SqlCommand com = new SqlCommand();
                com.CommandType = CommandType.Text;
                com.Connection = Conn;
                com.CommandText = comText;

                foreach (IDbDataParameter par in pars)
                {
                    com.Parameters.Add(par);
                }

                com.CommandTimeout = 1000;
                Open();
                com.ExecuteNonQuery();
                //Count = 0;
                return true;
            }
            catch (Exception ee)
            {
                errorMsg = ee.ToString();

                //if (Count < 1 && (ee.Message.IndexOf("open") != -1 || ee.Message.IndexOf("Open") != -1))
                //{
                //    Count++;
                //    return ExecuteParaNonExecuteDataSet(comText, ref errorMsg, pars);
                //}

                throw new Exception(errorMsg);
            }
            finally
            {
                if (AutoClose)
                {
                    Close();
                }

            }

            //Count = 0;
            //return true;
        }

        #endregion
    }
}
