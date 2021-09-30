using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace sybdj
{
    /// <summary>
    /// Summary description for Component.
    /// </summary>
    public class Component : DataModule
    {
        private ServiceManager serviceManager;
        private InfoCommand cmd;
        private InfoConnection infoConnection;
        private InfoCommand yb_bz06h;
        private InfoCommand yb_bztbd;
        private InfoCommand yb_ybbz;
        private InfoCommand yb_zy01h;
        private InfoCommand yb_dgys;
        private InfoCommand zc_cmd;
        private InfoConnection zc_infoConnection;
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
            Srvtools.Service service1 = new Srvtools.Service();
            Srvtools.Service service2 = new Srvtools.Service();
            Srvtools.Service service3 = new Srvtools.Service();
            Srvtools.KeyItem keyItem1 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem2 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem3 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem4 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem5 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem6 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem7 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem8 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem9 = new Srvtools.KeyItem();
            this.cmd = new Srvtools.InfoCommand();
            this.infoConnection = new Srvtools.InfoConnection();
            this.serviceManager = new Srvtools.ServiceManager();
            this.yb_bz06h = new Srvtools.InfoCommand();
            this.yb_bztbd = new Srvtools.InfoCommand();
            this.yb_ybbz = new Srvtools.InfoCommand();
            this.yb_zy01h = new Srvtools.InfoCommand();
            this.yb_dgys = new Srvtools.InfoCommand();
            this.zc_cmd = new Srvtools.InfoCommand();
            this.zc_infoConnection = new Srvtools.InfoConnection();
            ((System.ComponentModel.ISupportInitialize)(this.cmd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoConnection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bz06h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bztbd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybbz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zy01h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dgys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zc_cmd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zc_infoConnection)).BeginInit();
            // 
            // cmd
            // 
            this.cmd.CacheConnection = false;
            this.cmd.CommandText = "";
            this.cmd.CommandTimeout = 600;
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
            // 
            // yb_bz06h
            // 
            this.yb_bz06h.CacheConnection = false;
            this.yb_bz06h.CommandText = "select * from bz06h order by b6lyno asc";
            this.yb_bz06h.CommandTimeout = 30;
            this.yb_bz06h.CommandType = System.Data.CommandType.Text;
            this.yb_bz06h.DynamicTableName = false;
            this.yb_bz06h.EEPAlias = "Gocent";
            this.yb_bz06h.EncodingAfter = null;
            this.yb_bz06h.EncodingBefore = "Windows-1252";
            this.yb_bz06h.InfoConnection = this.infoConnection;
            keyItem1.KeyName = "b6comp";
            keyItem2.KeyName = "b6lyno";
            this.yb_bz06h.KeyFields.Add(keyItem1);
            this.yb_bz06h.KeyFields.Add(keyItem2);
            this.yb_bz06h.MultiSetWhere = false;
            this.yb_bz06h.Name = "yb_bz06h";
            this.yb_bz06h.NotificationAutoEnlist = false;
            this.yb_bz06h.SecExcept = null;
            this.yb_bz06h.SecFieldName = null;
            this.yb_bz06h.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_bz06h.SelectPaging = false;
            this.yb_bz06h.SelectTop = 0;
            this.yb_bz06h.SiteControl = false;
            this.yb_bz06h.SiteFieldName = null;
            this.yb_bz06h.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_bztbd
            // 
            this.yb_bztbd.CacheConnection = false;
            this.yb_bztbd.CommandText = "select bztbd.* from bztbd";
            this.yb_bztbd.CommandTimeout = 30;
            this.yb_bztbd.CommandType = System.Data.CommandType.Text;
            this.yb_bztbd.DynamicTableName = false;
            this.yb_bztbd.EEPAlias = "Gocent";
            this.yb_bztbd.EncodingAfter = null;
            this.yb_bztbd.EncodingBefore = "Windows-1252";
            this.yb_bztbd.InfoConnection = this.infoConnection;
            keyItem3.KeyName = "bzcomp";
            keyItem4.KeyName = "bzcodn";
            keyItem5.KeyName = "bzkeyx";
            this.yb_bztbd.KeyFields.Add(keyItem3);
            this.yb_bztbd.KeyFields.Add(keyItem4);
            this.yb_bztbd.KeyFields.Add(keyItem5);
            this.yb_bztbd.MultiSetWhere = false;
            this.yb_bztbd.Name = "yb_bztbd";
            this.yb_bztbd.NotificationAutoEnlist = false;
            this.yb_bztbd.SecExcept = null;
            this.yb_bztbd.SecFieldName = null;
            this.yb_bztbd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_bztbd.SelectPaging = false;
            this.yb_bztbd.SelectTop = 0;
            this.yb_bztbd.SiteControl = false;
            this.yb_bztbd.SiteFieldName = null;
            this.yb_bztbd.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ybbz
            // 
            this.yb_ybbz.CacheConnection = false;
            this.yb_ybbz.CommandText = "select dm,dmmc,id,pym,wbm,ybm,bz from ybbzmrdr order by len(dmmc) asc";
            this.yb_ybbz.CommandTimeout = 30;
            this.yb_ybbz.CommandType = System.Data.CommandType.Text;
            this.yb_ybbz.DynamicTableName = false;
            this.yb_ybbz.EEPAlias = null;
            this.yb_ybbz.EncodingAfter = null;
            this.yb_ybbz.EncodingBefore = "Windows-1252";
            this.yb_ybbz.InfoConnection = this.infoConnection;
            keyItem6.KeyName = "dm";
            this.yb_ybbz.KeyFields.Add(keyItem6);
            this.yb_ybbz.MultiSetWhere = false;
            this.yb_ybbz.Name = "yb_ybbz";
            this.yb_ybbz.NotificationAutoEnlist = false;
            this.yb_ybbz.SecExcept = null;
            this.yb_ybbz.SecFieldName = null;
            this.yb_ybbz.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_ybbz.SelectPaging = false;
            this.yb_ybbz.SelectTop = 0;
            this.yb_ybbz.SiteControl = false;
            this.yb_ybbz.SiteFieldName = null;
            this.yb_ybbz.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_zy01h
            // 
            this.yb_zy01h.CacheConnection = false;
            this.yb_zy01h.CommandText = "select * from zy01h \r\nwhere LEFT(zy01h.z1endv,1)in (\'0\',\'1\') ";
            this.yb_zy01h.CommandTimeout = 30;
            this.yb_zy01h.CommandType = System.Data.CommandType.Text;
            this.yb_zy01h.DynamicTableName = false;
            this.yb_zy01h.EEPAlias = null;
            this.yb_zy01h.EncodingAfter = null;
            this.yb_zy01h.EncodingBefore = "Windows-1252";
            this.yb_zy01h.InfoConnection = this.infoConnection;
            keyItem7.KeyName = "z1comp";
            keyItem8.KeyName = "z1zyno";
            keyItem9.KeyName = "z1ghno";
            this.yb_zy01h.KeyFields.Add(keyItem7);
            this.yb_zy01h.KeyFields.Add(keyItem8);
            this.yb_zy01h.KeyFields.Add(keyItem9);
            this.yb_zy01h.MultiSetWhere = false;
            this.yb_zy01h.Name = "yb_zy01h";
            this.yb_zy01h.NotificationAutoEnlist = false;
            this.yb_zy01h.SecExcept = null;
            this.yb_zy01h.SecFieldName = null;
            this.yb_zy01h.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_zy01h.SelectPaging = false;
            this.yb_zy01h.SelectTop = 0;
            this.yb_zy01h.SiteControl = false;
            this.yb_zy01h.SiteFieldName = null;
            this.yb_zy01h.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_dgys
            // 
            this.yb_dgys.CacheConnection = false;
            this.yb_dgys.CommandText = "select a.b1empn,b1name,case when isnull(b.dgysbm,\'\')=\'\' then a.b1empn else b.dgys" +
    "bm end as dgysbm,a.b1pymx,a.b1wbmx \r\nfrom bz01h a inner join ybdgyszd b on a.b1e" +
    "mpn=b.ysbm where a.b1type=1";
            this.yb_dgys.CommandTimeout = 30;
            this.yb_dgys.CommandType = System.Data.CommandType.Text;
            this.yb_dgys.DynamicTableName = false;
            this.yb_dgys.EEPAlias = null;
            this.yb_dgys.EncodingAfter = null;
            this.yb_dgys.EncodingBefore = "Windows-1252";
            this.yb_dgys.InfoConnection = this.infoConnection;
            this.yb_dgys.MultiSetWhere = false;
            this.yb_dgys.Name = "yb_dgys";
            this.yb_dgys.NotificationAutoEnlist = false;
            this.yb_dgys.SecExcept = null;
            this.yb_dgys.SecFieldName = null;
            this.yb_dgys.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_dgys.SelectPaging = false;
            this.yb_dgys.SelectTop = 0;
            this.yb_dgys.SiteControl = false;
            this.yb_dgys.SiteFieldName = null;
            this.yb_dgys.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // zc_cmd
            // 
            this.zc_cmd.CacheConnection = false;
            this.zc_cmd.CommandText = "";
            this.zc_cmd.CommandTimeout = 600;
            this.zc_cmd.CommandType = System.Data.CommandType.Text;
            this.zc_cmd.DynamicTableName = false;
            this.zc_cmd.EEPAlias = null;
            this.zc_cmd.EncodingAfter = null;
            this.zc_cmd.EncodingBefore = "Windows-1252";
            this.zc_cmd.InfoConnection = this.zc_infoConnection;
            this.zc_cmd.MultiSetWhere = false;
            this.zc_cmd.Name = "zc_cmd";
            this.zc_cmd.NotificationAutoEnlist = false;
            this.zc_cmd.SecExcept = null;
            this.zc_cmd.SecFieldName = "";
            this.zc_cmd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.zc_cmd.SelectPaging = false;
            this.zc_cmd.SelectTop = 0;
            this.zc_cmd.SiteControl = false;
            this.zc_cmd.SiteFieldName = "";
            this.zc_cmd.UpdatedRowSource = System.Data.UpdateRowSource.Both;
            // 
            // zc_infoConnection
            // 
            this.zc_infoConnection.EEPAlias = "Gocent_ZC";
            ((System.ComponentModel.ISupportInitialize)(this.cmd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoConnection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bz06h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bztbd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybbz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zy01h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dgys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zc_cmd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zc_infoConnection)).EndInit();

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
        /// <param>his项目编码,his项目名称,医保项目编码,医保项目名称,his拼音码,操作员代码,操作员姓名<param>
        /// <returns>1成功，0报错</returns>
        public object InsertHisYBDZProject(object[] objParam)
        {
            string hisxmbh = objParam[0].ToString();
            string hisxmmc = objParam[1].ToString();
            string ybxmbh = objParam[2].ToString();
            string ybxmmc = objParam[3].ToString();
            string hisPYM = objParam[4].ToString();
            string czydm = objParam[5].ToString();
            string czyxm = objParam[6].ToString();

            IDbConnection con1 = this.AllocateConnection(this.infoConnection.EEPAlias);

            if (con1.State == ConnectionState.Closed)
            {
                con1.Open();
            }

            IDbTransaction tran1 = con1.BeginTransaction();

            try
            {
                string sql = string.Format(@"insert into ybhisDZdr(hisxmbh, hisxmmc, ybxmbh, ybxmmc, gg
                    ,dw ,jxdm, jx, sfxmdjdm, sfxmdj, sflbdm, sflb, sfxmzldm, sfxmzl, czydm, czyxm, hisPYM) 
                    select top 1 '{0}', '{1}', '{2}', '{3}', gg, dw, jxdm, jx, sfxmdjdm, sfxmdj, sflbdm
                    , sflb, sfxmzldm, sfxmzl, '{4}', '{5}', '{6}' from ybmrdr where dm = '{2}'"
                    , hisxmbh, hisxmmc, ybxmbh, ybxmmc, czydm, czyxm, hisPYM);

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
