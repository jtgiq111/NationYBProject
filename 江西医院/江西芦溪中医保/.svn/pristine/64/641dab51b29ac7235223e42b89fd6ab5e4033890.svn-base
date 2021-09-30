using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace sybdjdr
{
    /// <summary>
    /// Summary description for Component.
    /// </summary>
    public class Component : DataModule
    {
        private ServiceManager serviceManager;
        private InfoCommand cmd;
        private InfoConnection infoConnection;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        public Component(System.ComponentModel.IContainer container)
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public Component()
        {
            ///
            /// This call is required by the Windows.Forms Designer.
            ///
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Srvtools.Service service1 = new Srvtools.Service();
            Srvtools.Service service2 = new Srvtools.Service();
            Srvtools.Service service3 = new Srvtools.Service();
            this.cmd = new Srvtools.InfoCommand(this.components);
            this.infoConnection = new Srvtools.InfoConnection(this.components);
            this.serviceManager = new Srvtools.ServiceManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cmd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoConnection)).BeginInit();
            // 
            // cmd
            // 
            this.cmd.CacheConnection = true;
            this.cmd.CommandText = "";
            this.cmd.CommandTimeout = 60;
            this.cmd.CommandType = System.Data.CommandType.Text;
            this.cmd.DynamicTableName = false;
            this.cmd.EEPAlias = null;
            this.cmd.EncodingAfter = null;
            this.cmd.EncodingBefore = "Windows-1252";
            this.cmd.InfoConnection = this.infoConnection;
            this.cmd.MultiSetWhere = false;
            this.cmd.Name = "cmd";
            this.cmd.NotificationAutoEnlist = false;
            this.cmd.SecExcept = null;
            this.cmd.SecFieldName = "";
            this.cmd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.cmd.SelectPaging = false;
            this.cmd.SelectTop = 0;
            this.cmd.SiteControl = false;
            this.cmd.SiteFieldName = "";
            this.cmd.UpdatedRowSource = System.Data.UpdateRowSource.Both;
            // 
            // infoConnection
            // 
            this.infoConnection.EEPAlias = "Gocent";
            // 
            // serviceManager
            // 
            service1.DelegateName = "BatExecuteSql";
            service1.NonLogin = false;
            service1.ServiceName = "BatExecuteSql";
            service2.DelegateName = "InsertHisYBDZProject";
            service2.NonLogin = false;
            service2.ServiceName = "InsertHisYBDZProject";
            service3.DelegateName = "DeleteHisYBDZProject";
            service3.NonLogin = false;
            service3.ServiceName = "DeleteHisYBDZProject";
            this.serviceManager.ServiceCollection.Add(service1);
            this.serviceManager.ServiceCollection.Add(service2);
            this.serviceManager.ServiceCollection.Add(service3);
            ((System.ComponentModel.ISupportInitialize)(this.cmd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoConnection)).EndInit();

        }

        #endregion

        #region 批量执行sql
        /// <summary>
        /// 批量执行
        /// </summary>
        /// <param name="objParam">sql语句</param>
        /// <returns>结果</returns>
        public object BatExecuteSql(object[] objParam)
        {
            string mess = "";

            IDbConnection con = this.AllocateConnection(this.infoConnection.EEPAlias);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            IDbTransaction tran = con.BeginTransaction();
            int row = 0;

            try
            {
                for (int i = 0; i < objParam.Length; i++)
                {
                    row = i;
                    this.ExecuteCommand(objParam[i].ToString(), con, tran);
                }

                tran.Commit();
                mess = "OK";
                this.ReleaseConnection(this.infoConnection.EEPAlias, con);
                return new object[] { 0, 1, mess };

            }
            catch
            {
                tran.Rollback();
                this.ReleaseConnection(this.infoConnection.EEPAlias, con);
                mess = "Error:" + objParam[row].ToString();
                return new object[] { 0, 0, mess };
            }
            finally
            {
                this.ReleaseConnection(this.infoConnection.EEPAlias, con);
            }

        }
        #endregion

        #region 对照模块
        /// <summary>
        /// 取消医院农保的对照项目
        /// </summary>
        /// <param>his项目编码,医保项目编码<param>
        /// <returns>1成功，0报错</returns>
        public object DeleteHisYBDZProject(object[] objParam)
        {
            string hisProjectCode = objParam[0].ToString();
            string YBProjectCode = objParam[1].ToString();

            StringBuilder retMsg = new StringBuilder(10000);

            IDbConnection con1 = this.AllocateConnection(this.infoConnection.EEPAlias);

            if (con1.State == ConnectionState.Closed)
            {
                con1.Open();
            }

            IDbTransaction tran1 = con1.BeginTransaction();

            try
            {

                string sql = string.Format("delete from ybhisdzdr where Hisxmbh = '{0}' and ybxmbh = '{1}'", hisProjectCode, YBProjectCode);
                int icount = this.ExecuteCommand(sql, con1, tran1);
                tran1.Commit();
                this.ReleaseConnection(this.infoConnection.EEPAlias, con1);
                return new object[] { 0, 1, "取消对照成功" };

            }
            catch (Exception error)
            {
                tran1.Rollback();
                this.ReleaseConnection(this.infoConnection.EEPAlias, con1);
                return new object[] { 0, 0, "Error:" + error.Message };
            }
            finally
            {
                this.ReleaseConnection(this.infoConnection.EEPAlias, con1);
            }
        }

        /// <summary>
        /// 插入医院医保的对照项目
        /// </summary>
        /// <param>his项目编码,his项目名称,医保项目编码,医保项目名称,his拼音码,操作员代码,操作员姓名,His价格<param>
        /// <returns>1成功，0报错</returns>
        public object InsertHisYBDZProject(object[] objParam)
        {
            string hisxmbh = objParam[0].ToString();  //his项目编码
            string hisxmmc = objParam[1].ToString();  //his项目名称 
            string ybxmbh = objParam[2].ToString();   //医保项目编码
            string ybxmmc = objParam[3].ToString();   //医保项目名称
            string hisPYM = objParam[4].ToString();   //his拼音码
            string czydm = objParam[5].ToString();    //操作员代码
            string czyxm = objParam[6].ToString();    //操作员姓名
            string hisprice = objParam[7].ToString(); //His价格

            IDbConnection con1 = this.AllocateConnection(this.infoConnection.EEPAlias);

            if (con1.State == ConnectionState.Closed)
            {
                con1.Open();
            }

            IDbTransaction tran1 = con1.BeginTransaction();

            try
            {

                //insert into ybhisDZdr(hisxmbh,hisxmmc,ybxmbh,ybxmmc,gg
                //    ,jxdm,sfxmdjdm,sflbdm,sfxmzldm,czydm,czyxm,hisPYM,yydj,jx,sfxmdj) 
                //    select top 1 '{0}',@hisxmmc,'{1}',@ybxmmc,gg,jxdm,sfxmdjdm,sflbdm,sfxmzldm,'{2}','{3}','{4}','{5}', jx,sfxmdj from ybmrdr where dm = '{1}' and bz = '1'"
                //    , hisProjectCode, YBProjectCode, "0000", "admin", hisPYM, hisprice

                string sql = string.Format(@"insert into ybhisDZdr(hisxmbh,hisxmmc,ybxmbh,ybxmmc,gg
                    ,jxdm,jx,sfxmdjdm ,sfxmdj,sflbdm,sfxmzldm,czydm,czyxm,hisPYM,yydj) 
                    select top 1 '{0}','{1}','{2}','{3}',gg,jxdm,jx,sfxmdjdm ,sfxmdj,sflbdm
                    ,sfxmzldm,'{4}','{5}','{6}','{7}' from ybmrdr where dm = '{2}'"
                    , hisxmbh, hisxmmc, ybxmbh, ybxmmc, czydm, czyxm, hisPYM,hisprice);

                int icount = this.ExecuteCommand(sql, con1, tran1);
                tran1.Commit();
                this.ReleaseConnection(this.infoConnection.EEPAlias, con1);
                return new object[] { 0, 1, "对照成功" };
            }
            catch (Exception error)
            {
                tran1.Rollback();
                this.ReleaseConnection(this.infoConnection.EEPAlias, con1);
                return new object[] { 0, 0, "Error:" + error.Message };
            }
            finally
            {
                this.ReleaseConnection(this.infoConnection.EEPAlias, con1);
            }

        }
        #endregion
    }
}
