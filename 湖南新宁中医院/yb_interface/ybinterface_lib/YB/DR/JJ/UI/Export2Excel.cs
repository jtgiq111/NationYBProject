using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace ybinterface_lib
{
    #region ��Ӱ����
    public class ColumnMap
    {
        private bool _usingColumnName = true;
        private int _columnIndex = -1;
        private string _columnName = null;
        private string _caption = null;

        /// <summary>
        /// ����Դ�������뵼��������Ӱ��
        /// </summary>
        /// <param name="columnName">Դ������</param>
        /// <param name="caption">��������</param>
        public ColumnMap(string columnName, string caption)
        {
            this._usingColumnName = true;
            this._columnName = columnName;
            this._caption = caption;
        }

        /// <summary>
        /// ����Դ������뵼��������Ӱ��
        /// </summary>
        /// <param name="columnIndex">Դ�����</param>
        /// <param name="caption">��������</param>
        public ColumnMap(int columnIndex, string caption)
        {
            this._usingColumnName = false;
            this._columnIndex = columnIndex;
            this._caption = caption;
        }

        /// <summary>
        /// �Ƿ�ʹ��������ָ��Դ��
        /// </summary>
        public bool usingColumnName
        {
            get
            {
                return _usingColumnName;
            }
        }

        /// <summary>
        /// Դ�����
        /// </summary>
        public int columnIndex
        {
            get
            {
                return this._columnIndex;
            }
        }

        /// <summary>
        /// ���뵽EXCEL����ʾ������
        /// </summary>
        public string caption
        {
            get
            {
                return this._caption;
            }
        }

        /// <summary>
        /// Դ������
        /// </summary>
        public string columnName
        {
            get
            {
                return this._columnName;
            }
        }
    }
    #endregion ��Ӱ����

    #region DtToXLS
    public class DataTableToExcel
    {
        /// <summary>
        /// �������ݱ�
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="sheetNo">excel���������</param>
        /// <param name="maps">��ӳ��</param>
        /// <returns>string</returns>
        private static string GetCreateTableSql(DataTable dt, int sheetNo, ColumnMap[] maps)
        {
            StringBuilder sbtable = new StringBuilder();
            int cols = maps.Length;
            sbtable.Append("CREATE TABLE ");
            sbtable.Append("[sheet" + sheetNo.ToString() + "] ( ");

            for (int i = 0; i < cols; i++)
            {
                string datatype;
                int num = dt.Rows.Count;
                //string j = dt.Columns["jzlsh"].ColumnName;

                switch (dt.Columns[maps[i].columnName].ColumnName.ToLower())
                {
                    case "float":
                    case "int32":
                    case "double":
                    case "decimal":
                    case "number": datatype = "float"; break;
                    //case "datetime": datatype = "datetime"; break;
                    default: datatype = "char(255)"; break;
                }

                if (i < cols - 1)
                {
                    sbtable.Append(string.Format("[{0}] {1},", maps[i].caption, datatype));
                }
                else
                {
                    sbtable.Append(string.Format("[{0}] {1})", maps[i].caption, datatype));
                }
            }

            return sbtable.ToString();
        }

        /// <summary>
        /// �������ݱ�����
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="sheetNo">excel���������</param>
        /// <param name="maps">��ӳ��</param>
        /// <returns>string</returns>
        private static string GetInsertSql(DataTable dt, int sheetNo, ColumnMap[] maps)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append("[sheet" + sheetNo.ToString() + "] ( ");
            int cols = maps.Length;

            for (int i = 0; i < cols; i++)
            {
                if (i < cols - 1)
                {
                    sb.Append("[" + maps[i].caption + "],");
                }
                else
                {
                    sb.Append("[" + maps[i].caption + "]) values (");
                }
            }

            for (int i = 0; i < cols; i++)
            {
                if (i < cols - 1)
                {
                    sb.Append("@" + maps[i].columnName.Replace("-", "") + ",");
                }
                else
                {
                    sb.Append("@" + maps[i].columnName.Replace("-", "") + ")");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// �����������ļ�
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="maps">��ӳ��</param>
        /// <param name="filepath">�ļ�·��</param>
        /// <param name="bOverwrite">�Ƿ񸲸�excel�ļ�</param>
        public static void Dt2File(DataTable dt, ColumnMap[] maps, string filepath, bool bOverwrite)
        {
            string extendname = Path.GetExtension(filepath);

            if (extendname == string.Empty || extendname.ToUpper() != ".XLS")
            {
                filepath += ".xls";
            }

            // �ļ��Ƿ����
            if (File.Exists(filepath) && !bOverwrite)
            {
                throw new Exception(filepath + " is exist.");
            }

            File.Delete(filepath);

            // У������Դ
            if (dt == null || dt.Rows.Count == 0)
            {
                throw new ArgumentNullException("Data source is not exist.");
            }

            #region дexcel�ļ�
            int rows = dt.Rows.Count;
            int cols = maps.Length;
            string tableName = dt.TableName;

            if (tableName == "")
            {
                tableName = "sheet";
            }

            int sheetNo = 1;
            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=YES;\"", filepath);
            //string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source={0}", filepath);

            using (OleDbConnection objConn = new OleDbConnection(connString))
            {
                objConn.Open();
                StringBuilder sb = new StringBuilder();
                OleDbTransaction tran = objConn.BeginTransaction();
                OleDbCommand objCmdT = new OleDbCommand();
                objCmdT.Connection = objConn;
                objCmdT.Transaction = tran;
                objCmdT.CommandText = GetCreateTableSql(dt, sheetNo, maps).Replace('.', ' ');
                objCmdT.ExecuteNonQuery();
                OleDbCommand objCmd = new OleDbCommand();
                objCmd.Connection = objConn;
                objCmd.Transaction = tran;
                objCmd.CommandText = GetInsertSql(dt, sheetNo, maps).Replace('.', ' ');
                OleDbParameterCollection param = objCmd.Parameters;

                for (int i = 0; i < cols; i++)
                {
                    if (param.Contains("@" + maps[i].columnName))
                    {
                        continue;
                    }

                    OleDbParameter onepar = new OleDbParameter();
                    onepar.Size = 255;
                    onepar.ParameterName = "@" + maps[i].columnName;
                    //switch (dt.Columns[maps[i].columnName].DataType.Name.ToLower())
                    //{
                    //    case "float": onepar.OleDbType = OleDbType.Single; break;
                    //    case "int32": onepar.OleDbType = OleDbType.Integer; break;
                    //    case "double": onepar.OleDbType = OleDbType.Double; break;
                    //    case "decimal": onepar.OleDbType = OleDbType.Decimal; break;
                    //    default: onepar.OleDbType = OleDbType.Char; break;
                    //}
                    onepar.OleDbType = OleDbType.VarChar;
                    param.Add(onepar);
                }

                int recordcount = 0;
                int round = 0;

                foreach (DataRow row in dt.Rows)
                {
                    round++;

                    for (int i = 0; i < param.Count; i++)
                    {
                        if (param[i].OleDbType == OleDbType.Numeric)
                        {
                            param[i].Value = 0;
                        }
                        else
                        {
                            param[i].Value = string.Empty;
                        }

                        if (row[maps[i].columnName] != DBNull.Value && row[maps[i].columnName] != null)
                        {
                            param[i].Value = row[maps[i].columnName];

                            if (row[maps[i].columnName].ToString().Length > 255)
                            {
                                param[i].Value = row[maps[i].columnName].ToString().Substring(0, 255);
                            }
                        }
                    }

                    objCmd.ExecuteNonQuery();
                    recordcount++;

                    if (recordcount > 65530)
                    {
                        tran.Commit();
                        recordcount = 0;
                        sheetNo++;
                        tran = objConn.BeginTransaction();
                        objCmdT = new OleDbCommand();
                        objCmdT.Connection = objConn;
                        objCmdT.Parameters.Clear();
                        objCmdT.Transaction = tran;
                        objCmdT.CommandText = GetCreateTableSql(dt, sheetNo, maps).Replace('.', ' ');
                        objCmdT.ExecuteNonQuery();
                        objCmd = new OleDbCommand();
                        objCmd.Connection = objConn;
                        objCmd.Transaction = tran;
                        param.Clear();
                        objCmd.CommandText = GetInsertSql(dt, sheetNo, maps).Replace('.', ' ');
                        param = objCmd.Parameters;

                        for (int i = 0; i < cols; i++)
                        {
                            if (param.Contains("@" + maps[i].columnName))
                            {
                                continue;
                            }

                            OleDbParameter onepar = new OleDbParameter();
                            onepar.Size = 255;
                            onepar.ParameterName = "@" + maps[i].columnName;
                            //switch (dt.Columns[maps[i].columnName].DataType.Name.ToLower())
                            //{
                            //    case "float": onepar.OleDbType = OleDbType.Single; break;
                            //    case "int32": onepar.OleDbType = OleDbType.Integer; break;
                            //    case "double": onepar.OleDbType = OleDbType.Double; break;
                            //    case "decimal": onepar.OleDbType = OleDbType.Decimal; break;
                            //    default: onepar.OleDbType = OleDbType.Char; break;
                            //}
                            onepar.OleDbType = OleDbType.VarChar;
                            param.Add(onepar);
                            //param.Add(new OleDbParameter("@" + columns[i], OleDbType.VarChar));
                        }//for
                    }//if                     

                }//for 
                tran.Commit();
            }
            #endregion
        }
    }
    #endregion

}
