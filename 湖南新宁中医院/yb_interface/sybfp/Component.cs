using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using Srvtools;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace sybfp
{
    /// <summary>
    /// Summary description for Component.
    /// </summary>
    public class Component : DataModule
    {
        private UpdateComponent ucMaster;
        private ServiceManager serviceManager;
        private InfoCommand cmd;
        private InfoConnection infoConnection;
        private InfoCommand bz05d;
        private InfoCommand bzfpjm;
        private InfoCommand yb_ybbz;
        private InfoCommand zlfs;
        private InfoCommand ybdgys;
        private InfoCommand ybmzzydjdr;
        private InfoCommand bztbd;
        private InfoCommand bz01h;
        private InfoCommand ybdgys_bz01h;
        private InfoCommand symzlb;
        private InfoCommand symzzd;
        private InfoCommand syzylb;
        private InfoCommand sycyzd;
        private InfoCommand bz01h_cwry;
        private InfoCommand tzybsylb;
        private InfoCommand tzsyrylb;
        private InfoCommand sfysyjh;
        private InfoCommand tzkflb;
        private InfoCommand tzsyscfs;
        private InfoCommand yb_zy01h;
        private InfoCommand yb_zy01h_fysc;
        private InfoCommand yb_bztbd;
        private InfoCommand yb_bz06h;
        private InfoCommand yb_sylb;
        private InfoCommand yb_sysslb;
        private InfoCommand yb_ssczmc;
        private InfoCommand yb_bz02d;
        private InfoCommand yb_gjks;
        private InfoCommand yb_dqbh;
        private InfoCommand yb_ybjbzd;
        private InfoCommand yb_yblyjg;
        private InfoCommand yb_zylx;
        private InfoCommand yb_mtbbz;
        private InfoCommand yb_dgyszd;
        private InfoCommand yb_ywsqlx;
        private InfoCommand yb_dbrgx;
        private InfoCommand yb_zjlx;
        private InfoCommand yb_bzmrdr;
        private InfoCommand yb_ybyljg;
        private InfoCommand yb_sqlx;
        private InfoCommand yb_ydrybz;
        private InfoCommand yb_dljsbz;
        private InfoCommand yb_cyjsbz;
        private InfoCommand yb_zflb;
        private InfoCommand yb_xzlx;

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
                return new object[] { 0, 1, mess };

            }
            catch
            {
                tran.Rollback();
                mess = "Error:" + objParam[row].ToString();
                return new object[] { 0, 0, mess };
            }
            finally
            {
                this.ReleaseConnection(this.infoConnection.EEPAlias, con);
            }

        }
        #endregion

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
            Srvtools.KeyItem keyItem1 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem2 = new Srvtools.KeyItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Component));
            Srvtools.KeyItem keyItem3 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem4 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem5 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem6 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem7 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem8 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem9 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem10 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem11 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem12 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem13 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem14 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem15 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem16 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem17 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem18 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem19 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem20 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem21 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem22 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem23 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem24 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem25 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem26 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem27 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem28 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem29 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem30 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem31 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem32 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem33 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem34 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem35 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem36 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem37 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem38 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem39 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem40 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem41 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem42 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem43 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem44 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem45 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem46 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem47 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem48 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem49 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem50 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem51 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem52 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem53 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem54 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem55 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem56 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem57 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem58 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem59 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem60 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem61 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem62 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem63 = new Srvtools.KeyItem();
            Srvtools.KeyItem keyItem64 = new Srvtools.KeyItem();
            this.ucMaster = new Srvtools.UpdateComponent(this.components);
            this.cmd = new Srvtools.InfoCommand(this.components);
            this.infoConnection = new Srvtools.InfoConnection(this.components);
            this.serviceManager = new Srvtools.ServiceManager(this.components);
            this.bz05d = new Srvtools.InfoCommand(this.components);
            this.bzfpjm = new Srvtools.InfoCommand(this.components);
            this.yb_ybbz = new Srvtools.InfoCommand(this.components);
            this.zlfs = new Srvtools.InfoCommand(this.components);
            this.ybdgys = new Srvtools.InfoCommand(this.components);
            this.ybmzzydjdr = new Srvtools.InfoCommand(this.components);
            this.bztbd = new Srvtools.InfoCommand(this.components);
            this.bz01h = new Srvtools.InfoCommand(this.components);
            this.ybdgys_bz01h = new Srvtools.InfoCommand(this.components);
            this.symzlb = new Srvtools.InfoCommand(this.components);
            this.symzzd = new Srvtools.InfoCommand(this.components);
            this.syzylb = new Srvtools.InfoCommand(this.components);
            this.sycyzd = new Srvtools.InfoCommand(this.components);
            this.bz01h_cwry = new Srvtools.InfoCommand(this.components);
            this.tzybsylb = new Srvtools.InfoCommand(this.components);
            this.tzsyrylb = new Srvtools.InfoCommand(this.components);
            this.sfysyjh = new Srvtools.InfoCommand(this.components);
            this.tzkflb = new Srvtools.InfoCommand(this.components);
            this.tzsyscfs = new Srvtools.InfoCommand(this.components);
            this.yb_zy01h = new Srvtools.InfoCommand(this.components);
            this.yb_zy01h_fysc = new Srvtools.InfoCommand(this.components);
            this.yb_bztbd = new Srvtools.InfoCommand(this.components);
            this.yb_bz06h = new Srvtools.InfoCommand(this.components);
            this.yb_sylb = new Srvtools.InfoCommand(this.components);
            this.yb_sysslb = new Srvtools.InfoCommand(this.components);
            this.yb_ssczmc = new Srvtools.InfoCommand(this.components);
            this.yb_bz02d = new Srvtools.InfoCommand(this.components);
            this.yb_gjks = new Srvtools.InfoCommand(this.components);
            this.yb_dqbh = new Srvtools.InfoCommand(this.components);
            this.yb_ybjbzd = new Srvtools.InfoCommand(this.components);
            this.yb_yblyjg = new Srvtools.InfoCommand(this.components);
            this.yb_zylx = new Srvtools.InfoCommand(this.components);
            this.yb_mtbbz = new Srvtools.InfoCommand(this.components);
            this.yb_dgyszd = new Srvtools.InfoCommand(this.components);
            this.yb_ywsqlx = new Srvtools.InfoCommand(this.components);
            this.yb_dbrgx = new Srvtools.InfoCommand(this.components);
            this.yb_zjlx = new Srvtools.InfoCommand(this.components);
            this.yb_bzmrdr = new Srvtools.InfoCommand(this.components);
            this.yb_ybyljg = new Srvtools.InfoCommand(this.components);
            this.yb_sqlx = new Srvtools.InfoCommand(this.components);
            this.yb_ydrybz = new Srvtools.InfoCommand(this.components);
            this.yb_dljsbz = new Srvtools.InfoCommand(this.components);
            this.yb_cyjsbz = new Srvtools.InfoCommand(this.components);
            this.yb_zflb = new Srvtools.InfoCommand(this.components);
            this.yb_xzlx = new Srvtools.InfoCommand(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cmd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoConnection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bz05d)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bzfpjm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybbz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zlfs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ybdgys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ybmzzydjdr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bztbd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bz01h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ybdgys_bz01h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symzlb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symzzd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.syzylb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sycyzd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bz01h_cwry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzybsylb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzsyrylb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sfysyjh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzkflb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzsyscfs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zy01h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zy01h_fysc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bztbd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bz06h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_sylb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_sysslb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ssczmc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bz02d)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_gjks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dqbh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybjbzd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_yblyjg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zylx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_mtbbz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dgyszd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ywsqlx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dbrgx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zjlx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bzmrdr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybyljg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_sqlx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ydrybz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dljsbz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_cyjsbz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zflb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_xzlx)).BeginInit();
            // 
            // ucMaster
            // 
            this.ucMaster.AutoTrans = true;
            this.ucMaster.ExceptJoin = false;
            this.ucMaster.LogInfo = null;
            this.ucMaster.Name = "ucMaster";
            this.ucMaster.RowAffectsCheck = true;
            this.ucMaster.SelectCmd = this.cmd;
            this.ucMaster.SelectCmdForUpdate = null;
            this.ucMaster.ServerModify = true;
            this.ucMaster.ServerModifyGetMax = false;
            this.ucMaster.TranscationScopeTimeOut = System.TimeSpan.Parse("00:02:00");
            this.ucMaster.TransIsolationLevel = System.Data.IsolationLevel.ReadCommitted;
            this.ucMaster.UseTranscationScope = false;
            this.ucMaster.WhereMode = Srvtools.WhereModeType.Keyfields;
            // 
            // cmd
            // 
            this.cmd.CacheConnection = false;
            this.cmd.CommandText = "";
            this.cmd.CommandTimeout = 100;
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
            this.serviceManager.ServiceCollection.Add(service1);
            // 
            // bz05d
            // 
            this.bz05d.CacheConnection = false;
            this.bz05d.CommandText = "select b5sfno,b5item,b5name,b5pymx,b5wbmx,b5ybmx,b5sfam from bz05d";
            this.bz05d.CommandTimeout = 30;
            this.bz05d.CommandType = System.Data.CommandType.Text;
            this.bz05d.DynamicTableName = false;
            this.bz05d.EEPAlias = null;
            this.bz05d.EncodingAfter = null;
            this.bz05d.EncodingBefore = "Windows-1252";
            this.bz05d.InfoConnection = this.infoConnection;
            this.bz05d.MultiSetWhere = false;
            this.bz05d.Name = "bz05d";
            this.bz05d.NotificationAutoEnlist = false;
            this.bz05d.SecExcept = null;
            this.bz05d.SecFieldName = null;
            this.bz05d.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.bz05d.SelectPaging = false;
            this.bz05d.SelectTop = 0;
            this.bz05d.SiteControl = false;
            this.bz05d.SiteFieldName = null;
            this.bz05d.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // bzfpjm
            // 
            this.bzfpjm.CacheConnection = false;
            this.bzfpjm.CommandText = "\t\t\tselect a.b5item,a.b5name,a.b5pymx,a.b5wbmx,a.b5ybmx,d.b5sfam from bzfpjm a\r\n\t\t" +
    "\t\t\t\t\tLEFT JOIN bz05d d on \ta.b5item=\td.b5item";
            this.bzfpjm.CommandTimeout = 30;
            this.bzfpjm.CommandType = System.Data.CommandType.Text;
            this.bzfpjm.DynamicTableName = false;
            this.bzfpjm.EEPAlias = null;
            this.bzfpjm.EncodingAfter = null;
            this.bzfpjm.EncodingBefore = "Windows-1252";
            this.bzfpjm.InfoConnection = this.infoConnection;
            this.bzfpjm.MultiSetWhere = false;
            this.bzfpjm.Name = "bzfpjm";
            this.bzfpjm.NotificationAutoEnlist = false;
            this.bzfpjm.SecExcept = null;
            this.bzfpjm.SecFieldName = null;
            this.bzfpjm.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.bzfpjm.SelectPaging = false;
            this.bzfpjm.SelectTop = 0;
            this.bzfpjm.SiteControl = false;
            this.bzfpjm.SiteFieldName = null;
            this.bzfpjm.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ybbz
            // 
            this.yb_ybbz.CacheConnection = false;
            this.yb_ybbz.CommandText = "select * from ybbzmrdr";
            this.yb_ybbz.CommandTimeout = 30;
            this.yb_ybbz.CommandType = System.Data.CommandType.Text;
            this.yb_ybbz.DynamicTableName = false;
            this.yb_ybbz.EEPAlias = null;
            this.yb_ybbz.EncodingAfter = null;
            this.yb_ybbz.EncodingBefore = "Windows-1252";
            this.yb_ybbz.InfoConnection = this.infoConnection;
            keyItem1.KeyName = "dm";
            keyItem2.KeyName = "yllb";
            this.yb_ybbz.KeyFields.Add(keyItem1);
            this.yb_ybbz.KeyFields.Add(keyItem2);
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
            // zlfs
            // 
            this.zlfs.CacheConnection = false;
            this.zlfs.CommandText = resources.GetString("zlfs.CommandText");
            this.zlfs.CommandTimeout = 30;
            this.zlfs.CommandType = System.Data.CommandType.Text;
            this.zlfs.DynamicTableName = false;
            this.zlfs.EEPAlias = null;
            this.zlfs.EncodingAfter = null;
            this.zlfs.EncodingBefore = "Windows-1252";
            this.zlfs.InfoConnection = this.infoConnection;
            this.zlfs.MultiSetWhere = false;
            this.zlfs.Name = "zlfs";
            this.zlfs.NotificationAutoEnlist = false;
            this.zlfs.SecExcept = null;
            this.zlfs.SecFieldName = null;
            this.zlfs.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.zlfs.SelectPaging = false;
            this.zlfs.SelectTop = 0;
            this.zlfs.SiteControl = false;
            this.zlfs.SiteFieldName = null;
            this.zlfs.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // ybdgys
            // 
            this.ybdgys.CacheConnection = false;
            this.ybdgys.CommandText = "with tmp as(select distinct ysxm,dgysbm,[dbo].[f_get_PY](ysxm,10) pym,[dbo].[f_ge" +
    "t_WB](ysxm,10) wbm from ybdgyszd)\r\nselect * from tmp";
            this.ybdgys.CommandTimeout = 30;
            this.ybdgys.CommandType = System.Data.CommandType.Text;
            this.ybdgys.DynamicTableName = false;
            this.ybdgys.EEPAlias = null;
            this.ybdgys.EncodingAfter = null;
            this.ybdgys.EncodingBefore = "Windows-1252";
            this.ybdgys.InfoConnection = this.infoConnection;
            this.ybdgys.MultiSetWhere = false;
            this.ybdgys.Name = "ybdgys";
            this.ybdgys.NotificationAutoEnlist = false;
            this.ybdgys.SecExcept = null;
            this.ybdgys.SecFieldName = null;
            this.ybdgys.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.ybdgys.SelectPaging = false;
            this.ybdgys.SelectTop = 0;
            this.ybdgys.SiteControl = false;
            this.ybdgys.SiteFieldName = null;
            this.ybdgys.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // ybmzzydjdr
            // 
            this.ybmzzydjdr.CacheConnection = false;
            this.ybmzzydjdr.CommandText = "select * from (select * from ybmzzydjdr where left(jzbz,1)=\'z\' and cxbz=\'1\')ybtb " +
    "\r\n                                              left join zy01h on zy01h.z1zyno=" +
    "ybtb.jzlsh  order by ybtb.kh  ";
            this.ybmzzydjdr.CommandTimeout = 30;
            this.ybmzzydjdr.CommandType = System.Data.CommandType.Text;
            this.ybmzzydjdr.DynamicTableName = false;
            this.ybmzzydjdr.EEPAlias = null;
            this.ybmzzydjdr.EncodingAfter = null;
            this.ybmzzydjdr.EncodingBefore = "Windows-1252";
            this.ybmzzydjdr.InfoConnection = this.infoConnection;
            this.ybmzzydjdr.MultiSetWhere = false;
            this.ybmzzydjdr.Name = "ybmzzydjdr";
            this.ybmzzydjdr.NotificationAutoEnlist = false;
            this.ybmzzydjdr.SecExcept = null;
            this.ybmzzydjdr.SecFieldName = null;
            this.ybmzzydjdr.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.ybmzzydjdr.SelectPaging = false;
            this.ybmzzydjdr.SelectTop = 0;
            this.ybmzzydjdr.SiteControl = false;
            this.ybmzzydjdr.SiteFieldName = null;
            this.ybmzzydjdr.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // bztbd
            // 
            this.bztbd.CacheConnection = false;
            this.bztbd.CommandText = "select * from bztbd where bzcodn=\'dq\'";
            this.bztbd.CommandTimeout = 30;
            this.bztbd.CommandType = System.Data.CommandType.Text;
            this.bztbd.DynamicTableName = false;
            this.bztbd.EEPAlias = null;
            this.bztbd.EncodingAfter = null;
            this.bztbd.EncodingBefore = "Windows-1252";
            this.bztbd.InfoConnection = this.infoConnection;
            keyItem3.KeyName = "bzcomp";
            keyItem4.KeyName = "bzcodn";
            keyItem5.KeyName = "bzkeyx";
            this.bztbd.KeyFields.Add(keyItem3);
            this.bztbd.KeyFields.Add(keyItem4);
            this.bztbd.KeyFields.Add(keyItem5);
            this.bztbd.MultiSetWhere = false;
            this.bztbd.Name = "bztbd";
            this.bztbd.NotificationAutoEnlist = false;
            this.bztbd.SecExcept = null;
            this.bztbd.SecFieldName = null;
            this.bztbd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.bztbd.SelectPaging = false;
            this.bztbd.SelectTop = 0;
            this.bztbd.SiteControl = false;
            this.bztbd.SiteFieldName = null;
            this.bztbd.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // bz01h
            // 
            this.bz01h.CacheConnection = false;
            this.bz01h.CommandText = "select * from bz01h";
            this.bz01h.CommandTimeout = 30;
            this.bz01h.CommandType = System.Data.CommandType.Text;
            this.bz01h.DynamicTableName = false;
            this.bz01h.EEPAlias = null;
            this.bz01h.EncodingAfter = null;
            this.bz01h.EncodingBefore = "Windows-1252";
            this.bz01h.InfoConnection = this.infoConnection;
            keyItem6.KeyName = "b1comp";
            keyItem7.KeyName = "b1empn";
            this.bz01h.KeyFields.Add(keyItem6);
            this.bz01h.KeyFields.Add(keyItem7);
            this.bz01h.MultiSetWhere = false;
            this.bz01h.Name = "bz01h";
            this.bz01h.NotificationAutoEnlist = false;
            this.bz01h.SecExcept = null;
            this.bz01h.SecFieldName = null;
            this.bz01h.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.bz01h.SelectPaging = false;
            this.bz01h.SelectTop = 0;
            this.bz01h.SiteControl = false;
            this.bz01h.SiteFieldName = null;
            this.bz01h.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // ybdgys_bz01h
            // 
            this.ybdgys_bz01h.CacheConnection = false;
            this.ybdgys_bz01h.CommandText = "\t\twith tmp as(select b1empn dgysbm,b1name ysxm,b1empn,b1name,b1pymx,b1wbmx,b1pymx" +
    " pym,b1wbmx wbm from bz01h where b1type=1)\r\n                select * from tmp";
            this.ybdgys_bz01h.CommandTimeout = 30;
            this.ybdgys_bz01h.CommandType = System.Data.CommandType.Text;
            this.ybdgys_bz01h.DynamicTableName = false;
            this.ybdgys_bz01h.EEPAlias = null;
            this.ybdgys_bz01h.EncodingAfter = null;
            this.ybdgys_bz01h.EncodingBefore = "Windows-1252";
            this.ybdgys_bz01h.InfoConnection = this.infoConnection;
            this.ybdgys_bz01h.MultiSetWhere = false;
            this.ybdgys_bz01h.Name = "ybdgys_bz01h";
            this.ybdgys_bz01h.NotificationAutoEnlist = false;
            this.ybdgys_bz01h.SecExcept = null;
            this.ybdgys_bz01h.SecFieldName = null;
            this.ybdgys_bz01h.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.ybdgys_bz01h.SelectPaging = false;
            this.ybdgys_bz01h.SelectTop = 0;
            this.ybdgys_bz01h.SiteControl = false;
            this.ybdgys_bz01h.SiteFieldName = null;
            this.ybdgys_bz01h.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // symzlb
            // 
            this.symzlb.CacheConnection = false;
            this.symzlb.CommandText = resources.GetString("symzlb.CommandText");
            this.symzlb.CommandTimeout = 30;
            this.symzlb.CommandType = System.Data.CommandType.Text;
            this.symzlb.DynamicTableName = false;
            this.symzlb.EEPAlias = null;
            this.symzlb.EncodingAfter = null;
            this.symzlb.EncodingBefore = "Windows-1252";
            this.symzlb.InfoConnection = this.infoConnection;
            this.symzlb.MultiSetWhere = false;
            this.symzlb.Name = "symzlb";
            this.symzlb.NotificationAutoEnlist = false;
            this.symzlb.SecExcept = null;
            this.symzlb.SecFieldName = null;
            this.symzlb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.symzlb.SelectPaging = false;
            this.symzlb.SelectTop = 0;
            this.symzlb.SiteControl = false;
            this.symzlb.SiteFieldName = null;
            this.symzlb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // symzzd
            // 
            this.symzzd.CacheConnection = false;
            this.symzzd.CommandText = resources.GetString("symzzd.CommandText");
            this.symzzd.CommandTimeout = 30;
            this.symzzd.CommandType = System.Data.CommandType.Text;
            this.symzzd.DynamicTableName = false;
            this.symzzd.EEPAlias = null;
            this.symzzd.EncodingAfter = null;
            this.symzzd.EncodingBefore = "Windows-1252";
            this.symzzd.InfoConnection = this.infoConnection;
            this.symzzd.MultiSetWhere = false;
            this.symzzd.Name = "symzzd";
            this.symzzd.NotificationAutoEnlist = false;
            this.symzzd.SecExcept = null;
            this.symzzd.SecFieldName = null;
            this.symzzd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.symzzd.SelectPaging = false;
            this.symzzd.SelectTop = 0;
            this.symzzd.SiteControl = false;
            this.symzzd.SiteFieldName = null;
            this.symzzd.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // syzylb
            // 
            this.syzylb.CacheConnection = false;
            this.syzylb.CommandText = resources.GetString("syzylb.CommandText");
            this.syzylb.CommandTimeout = 30;
            this.syzylb.CommandType = System.Data.CommandType.Text;
            this.syzylb.DynamicTableName = false;
            this.syzylb.EEPAlias = null;
            this.syzylb.EncodingAfter = null;
            this.syzylb.EncodingBefore = "Windows-1252";
            this.syzylb.InfoConnection = this.infoConnection;
            this.syzylb.MultiSetWhere = false;
            this.syzylb.Name = "syzylb";
            this.syzylb.NotificationAutoEnlist = false;
            this.syzylb.SecExcept = null;
            this.syzylb.SecFieldName = null;
            this.syzylb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.syzylb.SelectPaging = false;
            this.syzylb.SelectTop = 0;
            this.syzylb.SiteControl = false;
            this.syzylb.SiteFieldName = null;
            this.syzylb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // sycyzd
            // 
            this.sycyzd.CacheConnection = false;
            this.sycyzd.CommandText = resources.GetString("sycyzd.CommandText");
            this.sycyzd.CommandTimeout = 30;
            this.sycyzd.CommandType = System.Data.CommandType.Text;
            this.sycyzd.DynamicTableName = false;
            this.sycyzd.EEPAlias = null;
            this.sycyzd.EncodingAfter = null;
            this.sycyzd.EncodingBefore = "Windows-1252";
            this.sycyzd.InfoConnection = this.infoConnection;
            this.sycyzd.MultiSetWhere = false;
            this.sycyzd.Name = "sycyzd";
            this.sycyzd.NotificationAutoEnlist = false;
            this.sycyzd.SecExcept = null;
            this.sycyzd.SecFieldName = null;
            this.sycyzd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.sycyzd.SelectPaging = false;
            this.sycyzd.SelectTop = 0;
            this.sycyzd.SiteControl = false;
            this.sycyzd.SiteFieldName = null;
            this.sycyzd.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // bz01h_cwry
            // 
            this.bz01h_cwry.CacheConnection = false;
            this.bz01h_cwry.CommandText = "select * from bz01h where b1type=9";
            this.bz01h_cwry.CommandTimeout = 30;
            this.bz01h_cwry.CommandType = System.Data.CommandType.Text;
            this.bz01h_cwry.DynamicTableName = false;
            this.bz01h_cwry.EEPAlias = null;
            this.bz01h_cwry.EncodingAfter = null;
            this.bz01h_cwry.EncodingBefore = "Windows-1252";
            this.bz01h_cwry.InfoConnection = this.infoConnection;
            keyItem8.KeyName = "b1empn";
            this.bz01h_cwry.KeyFields.Add(keyItem8);
            this.bz01h_cwry.MultiSetWhere = false;
            this.bz01h_cwry.Name = "bz01h_cwry";
            this.bz01h_cwry.NotificationAutoEnlist = false;
            this.bz01h_cwry.SecExcept = null;
            this.bz01h_cwry.SecFieldName = null;
            this.bz01h_cwry.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.bz01h_cwry.SelectPaging = false;
            this.bz01h_cwry.SelectTop = 0;
            this.bz01h_cwry.SiteControl = false;
            this.bz01h_cwry.SiteFieldName = null;
            this.bz01h_cwry.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // tzybsylb
            // 
            this.tzybsylb.CacheConnection = false;
            this.tzybsylb.CommandText = resources.GetString("tzybsylb.CommandText");
            this.tzybsylb.CommandTimeout = 30;
            this.tzybsylb.CommandType = System.Data.CommandType.Text;
            this.tzybsylb.DynamicTableName = false;
            this.tzybsylb.EEPAlias = null;
            this.tzybsylb.EncodingAfter = null;
            this.tzybsylb.EncodingBefore = "Windows-1252";
            this.tzybsylb.InfoConnection = this.infoConnection;
            this.tzybsylb.MultiSetWhere = false;
            this.tzybsylb.Name = "tzybsylb";
            this.tzybsylb.NotificationAutoEnlist = false;
            this.tzybsylb.SecExcept = null;
            this.tzybsylb.SecFieldName = null;
            this.tzybsylb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.tzybsylb.SelectPaging = false;
            this.tzybsylb.SelectTop = 0;
            this.tzybsylb.SiteControl = false;
            this.tzybsylb.SiteFieldName = null;
            this.tzybsylb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // tzsyrylb
            // 
            this.tzsyrylb.CacheConnection = false;
            this.tzsyrylb.CommandText = resources.GetString("tzsyrylb.CommandText");
            this.tzsyrylb.CommandTimeout = 30;
            this.tzsyrylb.CommandType = System.Data.CommandType.Text;
            this.tzsyrylb.DynamicTableName = false;
            this.tzsyrylb.EEPAlias = null;
            this.tzsyrylb.EncodingAfter = null;
            this.tzsyrylb.EncodingBefore = "Windows-1252";
            this.tzsyrylb.InfoConnection = this.infoConnection;
            this.tzsyrylb.MultiSetWhere = false;
            this.tzsyrylb.Name = "tzsyrylb";
            this.tzsyrylb.NotificationAutoEnlist = false;
            this.tzsyrylb.SecExcept = null;
            this.tzsyrylb.SecFieldName = null;
            this.tzsyrylb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.tzsyrylb.SelectPaging = false;
            this.tzsyrylb.SelectTop = 0;
            this.tzsyrylb.SiteControl = false;
            this.tzsyrylb.SiteFieldName = null;
            this.tzsyrylb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // sfysyjh
            // 
            this.sfysyjh.CacheConnection = false;
            this.sfysyjh.CommandText = resources.GetString("sfysyjh.CommandText");
            this.sfysyjh.CommandTimeout = 30;
            this.sfysyjh.CommandType = System.Data.CommandType.Text;
            this.sfysyjh.DynamicTableName = false;
            this.sfysyjh.EEPAlias = null;
            this.sfysyjh.EncodingAfter = null;
            this.sfysyjh.EncodingBefore = "Windows-1252";
            this.sfysyjh.InfoConnection = this.infoConnection;
            this.sfysyjh.MultiSetWhere = false;
            this.sfysyjh.Name = "sfysyjh";
            this.sfysyjh.NotificationAutoEnlist = false;
            this.sfysyjh.SecExcept = null;
            this.sfysyjh.SecFieldName = null;
            this.sfysyjh.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.sfysyjh.SelectPaging = false;
            this.sfysyjh.SelectTop = 0;
            this.sfysyjh.SiteControl = false;
            this.sfysyjh.SiteFieldName = null;
            this.sfysyjh.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // tzkflb
            // 
            this.tzkflb.CacheConnection = false;
            this.tzkflb.CommandText = resources.GetString("tzkflb.CommandText");
            this.tzkflb.CommandTimeout = 30;
            this.tzkflb.CommandType = System.Data.CommandType.Text;
            this.tzkflb.DynamicTableName = false;
            this.tzkflb.EEPAlias = null;
            this.tzkflb.EncodingAfter = null;
            this.tzkflb.EncodingBefore = "Windows-1252";
            this.tzkflb.InfoConnection = this.infoConnection;
            this.tzkflb.MultiSetWhere = false;
            this.tzkflb.Name = "tzkflb";
            this.tzkflb.NotificationAutoEnlist = false;
            this.tzkflb.SecExcept = null;
            this.tzkflb.SecFieldName = null;
            this.tzkflb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.tzkflb.SelectPaging = false;
            this.tzkflb.SelectTop = 0;
            this.tzkflb.SiteControl = false;
            this.tzkflb.SiteFieldName = null;
            this.tzkflb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // tzsyscfs
            // 
            this.tzsyscfs.CacheConnection = false;
            this.tzsyscfs.CommandText = resources.GetString("tzsyscfs.CommandText");
            this.tzsyscfs.CommandTimeout = 30;
            this.tzsyscfs.CommandType = System.Data.CommandType.Text;
            this.tzsyscfs.DynamicTableName = false;
            this.tzsyscfs.EEPAlias = null;
            this.tzsyscfs.EncodingAfter = null;
            this.tzsyscfs.EncodingBefore = "Windows-1252";
            this.tzsyscfs.InfoConnection = this.infoConnection;
            this.tzsyscfs.MultiSetWhere = false;
            this.tzsyscfs.Name = "tzsyscfs";
            this.tzsyscfs.NotificationAutoEnlist = false;
            this.tzsyscfs.SecExcept = null;
            this.tzsyscfs.SecFieldName = null;
            this.tzsyscfs.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.tzsyscfs.SelectPaging = false;
            this.tzsyscfs.SelectTop = 0;
            this.tzsyscfs.SiteControl = false;
            this.tzsyscfs.SiteFieldName = null;
            this.tzsyscfs.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_zy01h
            // 
            this.yb_zy01h.CacheConnection = false;
            this.yb_zy01h.CommandText = resources.GetString("yb_zy01h.CommandText");
            this.yb_zy01h.CommandTimeout = 30;
            this.yb_zy01h.CommandType = System.Data.CommandType.Text;
            this.yb_zy01h.DynamicTableName = false;
            this.yb_zy01h.EEPAlias = null;
            this.yb_zy01h.EncodingAfter = null;
            this.yb_zy01h.EncodingBefore = "Windows-1252";
            this.yb_zy01h.InfoConnection = this.infoConnection;
            keyItem9.KeyName = "z1zyno";
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
            // yb_zy01h_fysc
            // 
            this.yb_zy01h_fysc.CacheConnection = false;
            this.yb_zy01h_fysc.CommandText = resources.GetString("yb_zy01h_fysc.CommandText");
            this.yb_zy01h_fysc.CommandTimeout = 30;
            this.yb_zy01h_fysc.CommandType = System.Data.CommandType.Text;
            this.yb_zy01h_fysc.DynamicTableName = false;
            this.yb_zy01h_fysc.EEPAlias = null;
            this.yb_zy01h_fysc.EncodingAfter = null;
            this.yb_zy01h_fysc.EncodingBefore = "Windows-1252";
            this.yb_zy01h_fysc.InfoConnection = this.infoConnection;
            keyItem10.KeyName = "z1zyno";
            this.yb_zy01h_fysc.KeyFields.Add(keyItem10);
            this.yb_zy01h_fysc.MultiSetWhere = false;
            this.yb_zy01h_fysc.Name = "yb_zy01h_fysc";
            this.yb_zy01h_fysc.NotificationAutoEnlist = false;
            this.yb_zy01h_fysc.SecExcept = null;
            this.yb_zy01h_fysc.SecFieldName = null;
            this.yb_zy01h_fysc.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_zy01h_fysc.SelectPaging = false;
            this.yb_zy01h_fysc.SelectTop = 0;
            this.yb_zy01h_fysc.SiteControl = false;
            this.yb_zy01h_fysc.SiteFieldName = null;
            this.yb_zy01h_fysc.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_bztbd
            // 
            this.yb_bztbd.CacheConnection = false;
            this.yb_bztbd.CommandText = "select * from bztbd where bzcodn=\'yl\' and bzusex=1 and bzmem3 like \'%Z%\' order by" +
    " bzybmx";
            this.yb_bztbd.CommandTimeout = 30;
            this.yb_bztbd.CommandType = System.Data.CommandType.Text;
            this.yb_bztbd.DynamicTableName = false;
            this.yb_bztbd.EEPAlias = null;
            this.yb_bztbd.EncodingAfter = null;
            this.yb_bztbd.EncodingBefore = "Windows-1252";
            this.yb_bztbd.InfoConnection = this.infoConnection;
            keyItem11.KeyName = "bzcomp";
            keyItem12.KeyName = "bzcodn";
            keyItem13.KeyName = "bzkeyx";
            this.yb_bztbd.KeyFields.Add(keyItem11);
            this.yb_bztbd.KeyFields.Add(keyItem12);
            this.yb_bztbd.KeyFields.Add(keyItem13);
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
            // yb_bz06h
            // 
            this.yb_bz06h.CacheConnection = false;
            this.yb_bz06h.CommandText = "select * from bz06h where b6lyjg like \'3%\' and b6chk1=1";
            this.yb_bz06h.CommandTimeout = 30;
            this.yb_bz06h.CommandType = System.Data.CommandType.Text;
            this.yb_bz06h.DynamicTableName = false;
            this.yb_bz06h.EEPAlias = null;
            this.yb_bz06h.EncodingAfter = null;
            this.yb_bz06h.EncodingBefore = "Windows-1252";
            this.yb_bz06h.InfoConnection = this.infoConnection;
            keyItem14.KeyName = "b6comp";
            keyItem15.KeyName = "b6lyno";
            this.yb_bz06h.KeyFields.Add(keyItem14);
            this.yb_bz06h.KeyFields.Add(keyItem15);
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
            // yb_sylb
            // 
            this.yb_sylb.CacheConnection = false;
            this.yb_sylb.CommandText = "select * from bztbd where bzcodn=\'matn_type\'";
            this.yb_sylb.CommandTimeout = 30;
            this.yb_sylb.CommandType = System.Data.CommandType.Text;
            this.yb_sylb.DynamicTableName = false;
            this.yb_sylb.EEPAlias = null;
            this.yb_sylb.EncodingAfter = null;
            this.yb_sylb.EncodingBefore = "Windows-1252";
            this.yb_sylb.InfoConnection = this.infoConnection;
            keyItem16.KeyName = "bzcomp";
            keyItem17.KeyName = "bzcodn";
            keyItem18.KeyName = "bzkeyx";
            this.yb_sylb.KeyFields.Add(keyItem16);
            this.yb_sylb.KeyFields.Add(keyItem17);
            this.yb_sylb.KeyFields.Add(keyItem18);
            this.yb_sylb.MultiSetWhere = false;
            this.yb_sylb.Name = "yb_sylb";
            this.yb_sylb.NotificationAutoEnlist = false;
            this.yb_sylb.SecExcept = null;
            this.yb_sylb.SecFieldName = null;
            this.yb_sylb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_sylb.SelectPaging = false;
            this.yb_sylb.SelectTop = 0;
            this.yb_sylb.SiteControl = false;
            this.yb_sylb.SiteFieldName = null;
            this.yb_sylb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_sysslb
            // 
            this.yb_sysslb.CacheConnection = false;
            this.yb_sysslb.CommandText = "select * from bztbd where bzcodn=\'birctrl_type\'";
            this.yb_sysslb.CommandTimeout = 30;
            this.yb_sysslb.CommandType = System.Data.CommandType.Text;
            this.yb_sysslb.DynamicTableName = false;
            this.yb_sysslb.EEPAlias = null;
            this.yb_sysslb.EncodingAfter = null;
            this.yb_sysslb.EncodingBefore = "Windows-1252";
            this.yb_sysslb.InfoConnection = this.infoConnection;
            keyItem19.KeyName = "bzcomp";
            keyItem20.KeyName = "bzcodn";
            keyItem21.KeyName = "bzkeyx";
            this.yb_sysslb.KeyFields.Add(keyItem19);
            this.yb_sysslb.KeyFields.Add(keyItem20);
            this.yb_sysslb.KeyFields.Add(keyItem21);
            this.yb_sysslb.MultiSetWhere = false;
            this.yb_sysslb.Name = "yb_sysslb";
            this.yb_sysslb.NotificationAutoEnlist = false;
            this.yb_sysslb.SecExcept = null;
            this.yb_sysslb.SecFieldName = null;
            this.yb_sysslb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_sysslb.SelectPaging = false;
            this.yb_sysslb.SelectTop = 0;
            this.yb_sysslb.SiteControl = false;
            this.yb_sysslb.SiteFieldName = null;
            this.yb_sysslb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ssczmc
            // 
            this.yb_ssczmc.CacheConnection = false;
            this.yb_ssczmc.CommandText = "select * from bztbd where bzcodn=\'ybss\'";
            this.yb_ssczmc.CommandTimeout = 30;
            this.yb_ssczmc.CommandType = System.Data.CommandType.Text;
            this.yb_ssczmc.DynamicTableName = false;
            this.yb_ssczmc.EEPAlias = null;
            this.yb_ssczmc.EncodingAfter = null;
            this.yb_ssczmc.EncodingBefore = "Windows-1252";
            this.yb_ssczmc.InfoConnection = this.infoConnection;
            keyItem22.KeyName = "bzcomp";
            keyItem23.KeyName = "bzcodn";
            keyItem24.KeyName = "bzkeyx";
            this.yb_ssczmc.KeyFields.Add(keyItem22);
            this.yb_ssczmc.KeyFields.Add(keyItem23);
            this.yb_ssczmc.KeyFields.Add(keyItem24);
            this.yb_ssczmc.MultiSetWhere = false;
            this.yb_ssczmc.Name = "yb_ssczmc";
            this.yb_ssczmc.NotificationAutoEnlist = false;
            this.yb_ssczmc.SecExcept = null;
            this.yb_ssczmc.SecFieldName = null;
            this.yb_ssczmc.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_ssczmc.SelectPaging = false;
            this.yb_ssczmc.SelectTop = 0;
            this.yb_ssczmc.SiteControl = false;
            this.yb_ssczmc.SiteFieldName = null;
            this.yb_ssczmc.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_bz02d
            // 
            this.yb_bz02d.CacheConnection = false;
            this.yb_bz02d.CommandText = "select * from bz02d";
            this.yb_bz02d.CommandTimeout = 30;
            this.yb_bz02d.CommandType = System.Data.CommandType.Text;
            this.yb_bz02d.DynamicTableName = false;
            this.yb_bz02d.EEPAlias = null;
            this.yb_bz02d.EncodingAfter = null;
            this.yb_bz02d.EncodingBefore = "Windows-1252";
            this.yb_bz02d.InfoConnection = this.infoConnection;
            keyItem25.KeyName = "b2ksno";
            keyItem26.KeyName = "b2ejks";
            this.yb_bz02d.KeyFields.Add(keyItem25);
            this.yb_bz02d.KeyFields.Add(keyItem26);
            this.yb_bz02d.MultiSetWhere = false;
            this.yb_bz02d.Name = "yb_bz02d";
            this.yb_bz02d.NotificationAutoEnlist = false;
            this.yb_bz02d.SecExcept = null;
            this.yb_bz02d.SecFieldName = null;
            this.yb_bz02d.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_bz02d.SelectPaging = false;
            this.yb_bz02d.SelectTop = 0;
            this.yb_bz02d.SiteControl = false;
            this.yb_bz02d.SiteFieldName = null;
            this.yb_bz02d.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_gjks
            // 
            this.yb_gjks.CacheConnection = false;
            this.yb_gjks.CommandText = "select * from bztbd where bzcodn=\'dept\'";
            this.yb_gjks.CommandTimeout = 30;
            this.yb_gjks.CommandType = System.Data.CommandType.Text;
            this.yb_gjks.DynamicTableName = false;
            this.yb_gjks.EEPAlias = null;
            this.yb_gjks.EncodingAfter = null;
            this.yb_gjks.EncodingBefore = "Windows-1252";
            this.yb_gjks.InfoConnection = this.infoConnection;
            keyItem27.KeyName = "bzcomp";
            keyItem28.KeyName = "bzcodn";
            keyItem29.KeyName = "bzkeyx";
            this.yb_gjks.KeyFields.Add(keyItem27);
            this.yb_gjks.KeyFields.Add(keyItem28);
            this.yb_gjks.KeyFields.Add(keyItem29);
            this.yb_gjks.MultiSetWhere = false;
            this.yb_gjks.Name = "yb_gjks";
            this.yb_gjks.NotificationAutoEnlist = false;
            this.yb_gjks.SecExcept = null;
            this.yb_gjks.SecFieldName = null;
            this.yb_gjks.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_gjks.SelectPaging = false;
            this.yb_gjks.SelectTop = 0;
            this.yb_gjks.SiteControl = false;
            this.yb_gjks.SiteFieldName = null;
            this.yb_gjks.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_dqbh
            // 
            this.yb_dqbh.CacheConnection = false;
            this.yb_dqbh.CommandText = "select * from bztbd where bzcodn=\'dq\'";
            this.yb_dqbh.CommandTimeout = 30;
            this.yb_dqbh.CommandType = System.Data.CommandType.Text;
            this.yb_dqbh.DynamicTableName = false;
            this.yb_dqbh.EEPAlias = null;
            this.yb_dqbh.EncodingAfter = null;
            this.yb_dqbh.EncodingBefore = "Windows-1252";
            this.yb_dqbh.InfoConnection = this.infoConnection;
            keyItem30.KeyName = "bzcomp";
            keyItem31.KeyName = "bzcodn";
            keyItem32.KeyName = "bzkeyx";
            this.yb_dqbh.KeyFields.Add(keyItem30);
            this.yb_dqbh.KeyFields.Add(keyItem31);
            this.yb_dqbh.KeyFields.Add(keyItem32);
            this.yb_dqbh.MultiSetWhere = false;
            this.yb_dqbh.Name = "yb_dqbh";
            this.yb_dqbh.NotificationAutoEnlist = false;
            this.yb_dqbh.SecExcept = null;
            this.yb_dqbh.SecFieldName = null;
            this.yb_dqbh.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_dqbh.SelectPaging = false;
            this.yb_dqbh.SelectTop = 0;
            this.yb_dqbh.SiteControl = false;
            this.yb_dqbh.SiteFieldName = null;
            this.yb_dqbh.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ybjbzd
            // 
            this.yb_ybjbzd.CacheConnection = false;
            this.yb_ybjbzd.CommandText = "select * from ybbzmrdr where yllb=\'11\'";
            this.yb_ybjbzd.CommandTimeout = 30;
            this.yb_ybjbzd.CommandType = System.Data.CommandType.Text;
            this.yb_ybjbzd.DynamicTableName = false;
            this.yb_ybjbzd.EEPAlias = null;
            this.yb_ybjbzd.EncodingAfter = null;
            this.yb_ybjbzd.EncodingBefore = "Windows-1252";
            this.yb_ybjbzd.InfoConnection = this.infoConnection;
            keyItem33.KeyName = "dm";
            this.yb_ybjbzd.KeyFields.Add(keyItem33);
            this.yb_ybjbzd.MultiSetWhere = false;
            this.yb_ybjbzd.Name = "yb_ybjbzd";
            this.yb_ybjbzd.NotificationAutoEnlist = false;
            this.yb_ybjbzd.SecExcept = null;
            this.yb_ybjbzd.SecFieldName = null;
            this.yb_ybjbzd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_ybjbzd.SelectPaging = false;
            this.yb_ybjbzd.SelectTop = 0;
            this.yb_ybjbzd.SiteControl = false;
            this.yb_ybjbzd.SiteFieldName = null;
            this.yb_ybjbzd.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_yblyjg
            // 
            this.yb_yblyjg.CacheConnection = false;
            this.yb_yblyjg.CommandText = "select * from YBDDYLJGXX ";
            this.yb_yblyjg.CommandTimeout = 30;
            this.yb_yblyjg.CommandType = System.Data.CommandType.Text;
            this.yb_yblyjg.DynamicTableName = false;
            this.yb_yblyjg.EEPAlias = null;
            this.yb_yblyjg.EncodingAfter = null;
            this.yb_yblyjg.EncodingBefore = "Windows-1252";
            this.yb_yblyjg.InfoConnection = this.infoConnection;
            keyItem34.KeyName = "ddyljgbh";
            this.yb_yblyjg.KeyFields.Add(keyItem34);
            this.yb_yblyjg.MultiSetWhere = false;
            this.yb_yblyjg.Name = "yb_yblyjg";
            this.yb_yblyjg.NotificationAutoEnlist = false;
            this.yb_yblyjg.SecExcept = null;
            this.yb_yblyjg.SecFieldName = null;
            this.yb_yblyjg.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_yblyjg.SelectPaging = false;
            this.yb_yblyjg.SelectTop = 0;
            this.yb_yblyjg.SiteControl = false;
            this.yb_yblyjg.SiteFieldName = null;
            this.yb_yblyjg.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_zylx
            // 
            this.yb_zylx.CacheConnection = false;
            this.yb_zylx.CommandText = "select * from bztbd where bzcodn=\'refl_type\'";
            this.yb_zylx.CommandTimeout = 30;
            this.yb_zylx.CommandType = System.Data.CommandType.Text;
            this.yb_zylx.DynamicTableName = false;
            this.yb_zylx.EEPAlias = null;
            this.yb_zylx.EncodingAfter = null;
            this.yb_zylx.EncodingBefore = "Windows-1252";
            this.yb_zylx.InfoConnection = this.infoConnection;
            keyItem35.KeyName = "bzcomp";
            keyItem36.KeyName = "bzcodn";
            keyItem37.KeyName = "bzkeyx";
            this.yb_zylx.KeyFields.Add(keyItem35);
            this.yb_zylx.KeyFields.Add(keyItem36);
            this.yb_zylx.KeyFields.Add(keyItem37);
            this.yb_zylx.MultiSetWhere = false;
            this.yb_zylx.Name = "yb_zylx";
            this.yb_zylx.NotificationAutoEnlist = false;
            this.yb_zylx.SecExcept = null;
            this.yb_zylx.SecFieldName = null;
            this.yb_zylx.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_zylx.SelectPaging = false;
            this.yb_zylx.SelectTop = 0;
            this.yb_zylx.SiteControl = false;
            this.yb_zylx.SiteFieldName = null;
            this.yb_zylx.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_mtbbz
            // 
            this.yb_mtbbz.CacheConnection = false;
            this.yb_mtbbz.CommandText = "select * from ybbzmrdr where yllb in(\'12\',\'13\')";
            this.yb_mtbbz.CommandTimeout = 30;
            this.yb_mtbbz.CommandType = System.Data.CommandType.Text;
            this.yb_mtbbz.DynamicTableName = false;
            this.yb_mtbbz.EEPAlias = null;
            this.yb_mtbbz.EncodingAfter = null;
            this.yb_mtbbz.EncodingBefore = "Windows-1252";
            this.yb_mtbbz.InfoConnection = this.infoConnection;
            keyItem38.KeyName = "dm";
            this.yb_mtbbz.KeyFields.Add(keyItem38);
            this.yb_mtbbz.MultiSetWhere = false;
            this.yb_mtbbz.Name = "yb_mtbbz";
            this.yb_mtbbz.NotificationAutoEnlist = false;
            this.yb_mtbbz.SecExcept = null;
            this.yb_mtbbz.SecFieldName = null;
            this.yb_mtbbz.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_mtbbz.SelectPaging = false;
            this.yb_mtbbz.SelectTop = 0;
            this.yb_mtbbz.SiteControl = false;
            this.yb_mtbbz.SiteFieldName = null;
            this.yb_mtbbz.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_dgyszd
            // 
            this.yb_dgyszd.CacheConnection = false;
            this.yb_dgyszd.CommandText = "select * from ybdgyszd";
            this.yb_dgyszd.CommandTimeout = 30;
            this.yb_dgyszd.CommandType = System.Data.CommandType.Text;
            this.yb_dgyszd.DynamicTableName = false;
            this.yb_dgyszd.EEPAlias = null;
            this.yb_dgyszd.EncodingAfter = null;
            this.yb_dgyszd.EncodingBefore = "Windows-1252";
            this.yb_dgyszd.InfoConnection = this.infoConnection;
            keyItem39.KeyName = "ysbm";
            this.yb_dgyszd.KeyFields.Add(keyItem39);
            this.yb_dgyszd.MultiSetWhere = false;
            this.yb_dgyszd.Name = "yb_dgyszd";
            this.yb_dgyszd.NotificationAutoEnlist = false;
            this.yb_dgyszd.SecExcept = null;
            this.yb_dgyszd.SecFieldName = null;
            this.yb_dgyszd.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_dgyszd.SelectPaging = false;
            this.yb_dgyszd.SelectTop = 0;
            this.yb_dgyszd.SiteControl = false;
            this.yb_dgyszd.SiteFieldName = null;
            this.yb_dgyszd.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ywsqlx
            // 
            this.yb_ywsqlx.CacheConnection = false;
            this.yb_ywsqlx.CommandText = "select * from bztbd where bzcodn=\'biz_appy_type\'";
            this.yb_ywsqlx.CommandTimeout = 30;
            this.yb_ywsqlx.CommandType = System.Data.CommandType.Text;
            this.yb_ywsqlx.DynamicTableName = false;
            this.yb_ywsqlx.EEPAlias = null;
            this.yb_ywsqlx.EncodingAfter = null;
            this.yb_ywsqlx.EncodingBefore = "Windows-1252";
            this.yb_ywsqlx.InfoConnection = this.infoConnection;
            keyItem40.KeyName = "bzcomp";
            keyItem41.KeyName = "bzcodn";
            keyItem42.KeyName = "bzkeyx";
            this.yb_ywsqlx.KeyFields.Add(keyItem40);
            this.yb_ywsqlx.KeyFields.Add(keyItem41);
            this.yb_ywsqlx.KeyFields.Add(keyItem42);
            this.yb_ywsqlx.MultiSetWhere = false;
            this.yb_ywsqlx.Name = "yb_ywsqlx";
            this.yb_ywsqlx.NotificationAutoEnlist = false;
            this.yb_ywsqlx.SecExcept = null;
            this.yb_ywsqlx.SecFieldName = null;
            this.yb_ywsqlx.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_ywsqlx.SelectPaging = false;
            this.yb_ywsqlx.SelectTop = 0;
            this.yb_ywsqlx.SiteControl = false;
            this.yb_ywsqlx.SiteFieldName = null;
            this.yb_ywsqlx.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_dbrgx
            // 
            this.yb_dbrgx.CacheConnection = false;
            this.yb_dbrgx.CommandText = "select * from bztbd where bzcodn=\'agnter_rlts\'";
            this.yb_dbrgx.CommandTimeout = 30;
            this.yb_dbrgx.CommandType = System.Data.CommandType.Text;
            this.yb_dbrgx.DynamicTableName = false;
            this.yb_dbrgx.EEPAlias = null;
            this.yb_dbrgx.EncodingAfter = null;
            this.yb_dbrgx.EncodingBefore = "Windows-1252";
            this.yb_dbrgx.InfoConnection = this.infoConnection;
            keyItem43.KeyName = "bzcomp";
            keyItem44.KeyName = "bzcodn";
            keyItem45.KeyName = "bzkeyx";
            this.yb_dbrgx.KeyFields.Add(keyItem43);
            this.yb_dbrgx.KeyFields.Add(keyItem44);
            this.yb_dbrgx.KeyFields.Add(keyItem45);
            this.yb_dbrgx.MultiSetWhere = false;
            this.yb_dbrgx.Name = "yb_dbrgx";
            this.yb_dbrgx.NotificationAutoEnlist = false;
            this.yb_dbrgx.SecExcept = null;
            this.yb_dbrgx.SecFieldName = null;
            this.yb_dbrgx.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_dbrgx.SelectPaging = false;
            this.yb_dbrgx.SelectTop = 0;
            this.yb_dbrgx.SiteControl = false;
            this.yb_dbrgx.SiteFieldName = null;
            this.yb_dbrgx.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_zjlx
            // 
            this.yb_zjlx.CacheConnection = false;
            this.yb_zjlx.CommandText = "select * from bztbd where bzcodn=\'psn_cert_type\'";
            this.yb_zjlx.CommandTimeout = 30;
            this.yb_zjlx.CommandType = System.Data.CommandType.Text;
            this.yb_zjlx.DynamicTableName = false;
            this.yb_zjlx.EEPAlias = null;
            this.yb_zjlx.EncodingAfter = null;
            this.yb_zjlx.EncodingBefore = "Windows-1252";
            this.yb_zjlx.InfoConnection = this.infoConnection;
            keyItem46.KeyName = "bzcomp";
            keyItem47.KeyName = "bzcodn";
            keyItem48.KeyName = "bzkeyx";
            this.yb_zjlx.KeyFields.Add(keyItem46);
            this.yb_zjlx.KeyFields.Add(keyItem47);
            this.yb_zjlx.KeyFields.Add(keyItem48);
            this.yb_zjlx.MultiSetWhere = false;
            this.yb_zjlx.Name = "yb_zjlx";
            this.yb_zjlx.NotificationAutoEnlist = false;
            this.yb_zjlx.SecExcept = null;
            this.yb_zjlx.SecFieldName = null;
            this.yb_zjlx.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_zjlx.SelectPaging = false;
            this.yb_zjlx.SelectTop = 0;
            this.yb_zjlx.SiteControl = false;
            this.yb_zjlx.SiteFieldName = null;
            this.yb_zjlx.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_bzmrdr
            // 
            this.yb_bzmrdr.CacheConnection = false;
            this.yb_bzmrdr.CommandText = "select * from ybbzmrdr";
            this.yb_bzmrdr.CommandTimeout = 30;
            this.yb_bzmrdr.CommandType = System.Data.CommandType.Text;
            this.yb_bzmrdr.DynamicTableName = false;
            this.yb_bzmrdr.EEPAlias = null;
            this.yb_bzmrdr.EncodingAfter = null;
            this.yb_bzmrdr.EncodingBefore = "Windows-1252";
            this.yb_bzmrdr.InfoConnection = this.infoConnection;
            keyItem49.KeyName = "dm";
            this.yb_bzmrdr.KeyFields.Add(keyItem49);
            this.yb_bzmrdr.MultiSetWhere = false;
            this.yb_bzmrdr.Name = "yb_bzmrdr";
            this.yb_bzmrdr.NotificationAutoEnlist = false;
            this.yb_bzmrdr.SecExcept = null;
            this.yb_bzmrdr.SecFieldName = null;
            this.yb_bzmrdr.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_bzmrdr.SelectPaging = false;
            this.yb_bzmrdr.SelectTop = 0;
            this.yb_bzmrdr.SiteControl = false;
            this.yb_bzmrdr.SiteFieldName = null;
            this.yb_bzmrdr.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ybyljg
            // 
            this.yb_ybyljg.CacheConnection = false;
            this.yb_ybyljg.CommandText = "SELECT * FROM BZTBD WHERE   bzcodn=\'AKA020\'  and bzusex =1";
            this.yb_ybyljg.CommandTimeout = 30;
            this.yb_ybyljg.CommandType = System.Data.CommandType.Text;
            this.yb_ybyljg.DynamicTableName = false;
            this.yb_ybyljg.EEPAlias = null;
            this.yb_ybyljg.EncodingAfter = null;
            this.yb_ybyljg.EncodingBefore = "Windows-1252";
            this.yb_ybyljg.InfoConnection = this.infoConnection;
            keyItem50.KeyName = "bzcomp";
            keyItem51.KeyName = "bzcodn";
            keyItem52.KeyName = "bzkeyx";
            this.yb_ybyljg.KeyFields.Add(keyItem50);
            this.yb_ybyljg.KeyFields.Add(keyItem51);
            this.yb_ybyljg.KeyFields.Add(keyItem52);
            this.yb_ybyljg.MultiSetWhere = false;
            this.yb_ybyljg.Name = "yb_ybyljg";
            this.yb_ybyljg.NotificationAutoEnlist = false;
            this.yb_ybyljg.SecExcept = null;
            this.yb_ybyljg.SecFieldName = null;
            this.yb_ybyljg.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_ybyljg.SelectPaging = false;
            this.yb_ybyljg.SelectTop = 0;
            this.yb_ybyljg.SiteControl = false;
            this.yb_ybyljg.SiteFieldName = null;
            this.yb_ybyljg.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_sqlx
            // 
            this.yb_sqlx.CacheConnection = false;
            this.yb_sqlx.CommandText = "SELECT * FROM BZTBD WHERE   bzcodn=\'AKA230\'  and bzusex =1";
            this.yb_sqlx.CommandTimeout = 30;
            this.yb_sqlx.CommandType = System.Data.CommandType.Text;
            this.yb_sqlx.DynamicTableName = false;
            this.yb_sqlx.EEPAlias = null;
            this.yb_sqlx.EncodingAfter = null;
            this.yb_sqlx.EncodingBefore = "Windows-1252";
            this.yb_sqlx.InfoConnection = this.infoConnection;
            keyItem53.KeyName = "bzcomp";
            keyItem54.KeyName = "bzcodn";
            keyItem55.KeyName = "bzkeyx";
            this.yb_sqlx.KeyFields.Add(keyItem53);
            this.yb_sqlx.KeyFields.Add(keyItem54);
            this.yb_sqlx.KeyFields.Add(keyItem55);
            this.yb_sqlx.MultiSetWhere = false;
            this.yb_sqlx.Name = "yb_sqlx";
            this.yb_sqlx.NotificationAutoEnlist = false;
            this.yb_sqlx.SecExcept = null;
            this.yb_sqlx.SecFieldName = null;
            this.yb_sqlx.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_sqlx.SelectPaging = false;
            this.yb_sqlx.SelectTop = 0;
            this.yb_sqlx.SiteControl = false;
            this.yb_sqlx.SiteFieldName = null;
            this.yb_sqlx.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_ydrybz
            // 
            this.yb_ydrybz.CacheConnection = false;
            this.yb_ydrybz.CommandText = "with tmp as(\r\nselect \'0\' dm,\'本地\' dmmc\r\nunion all\r\nselect \'1\' dm,\'异地\' dmmc\r\n)selec" +
    "t * from tmp\r\n";
            this.yb_ydrybz.CommandTimeout = 30;
            this.yb_ydrybz.CommandType = System.Data.CommandType.Text;
            this.yb_ydrybz.DynamicTableName = false;
            this.yb_ydrybz.EEPAlias = null;
            this.yb_ydrybz.EncodingAfter = null;
            this.yb_ydrybz.EncodingBefore = "Windows-1252";
            this.yb_ydrybz.InfoConnection = this.infoConnection;
            this.yb_ydrybz.MultiSetWhere = false;
            this.yb_ydrybz.Name = "yb_ydrybz";
            this.yb_ydrybz.NotificationAutoEnlist = false;
            this.yb_ydrybz.SecExcept = null;
            this.yb_ydrybz.SecFieldName = null;
            this.yb_ydrybz.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_ydrybz.SelectPaging = false;
            this.yb_ydrybz.SelectTop = 0;
            this.yb_ydrybz.SiteControl = false;
            this.yb_ydrybz.SiteFieldName = null;
            this.yb_ydrybz.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_dljsbz
            // 
            this.yb_dljsbz.CacheConnection = false;
            this.yb_dljsbz.CommandText = "SELECT * FROM bztbd WHERE   bzcodn=\'AKA079\'  and bzusex =1";
            this.yb_dljsbz.CommandTimeout = 30;
            this.yb_dljsbz.CommandType = System.Data.CommandType.Text;
            this.yb_dljsbz.DynamicTableName = false;
            this.yb_dljsbz.EEPAlias = null;
            this.yb_dljsbz.EncodingAfter = null;
            this.yb_dljsbz.EncodingBefore = "Windows-1252";
            this.yb_dljsbz.InfoConnection = this.infoConnection;
            keyItem56.KeyName = "bzcomp";
            keyItem57.KeyName = "bzcodn";
            keyItem58.KeyName = "bzkeyx";
            this.yb_dljsbz.KeyFields.Add(keyItem56);
            this.yb_dljsbz.KeyFields.Add(keyItem57);
            this.yb_dljsbz.KeyFields.Add(keyItem58);
            this.yb_dljsbz.MultiSetWhere = false;
            this.yb_dljsbz.Name = "yb_dljsbz";
            this.yb_dljsbz.NotificationAutoEnlist = false;
            this.yb_dljsbz.SecExcept = null;
            this.yb_dljsbz.SecFieldName = null;
            this.yb_dljsbz.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_dljsbz.SelectPaging = false;
            this.yb_dljsbz.SelectTop = 0;
            this.yb_dljsbz.SiteControl = false;
            this.yb_dljsbz.SiteFieldName = null;
            this.yb_dljsbz.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_cyjsbz
            // 
            this.yb_cyjsbz.CacheConnection = false;
            this.yb_cyjsbz.CommandText = "\r\nwith tmp as(\r\nselect \'0\' dm,\'未结算\' dmmc\r\nunion all\r\nselect \'1\' dm,\'已结算\' dmmc\r\n)s" +
    "elect * from tmp\r\n";
            this.yb_cyjsbz.CommandTimeout = 30;
            this.yb_cyjsbz.CommandType = System.Data.CommandType.Text;
            this.yb_cyjsbz.DynamicTableName = false;
            this.yb_cyjsbz.EEPAlias = null;
            this.yb_cyjsbz.EncodingAfter = null;
            this.yb_cyjsbz.EncodingBefore = "Windows-1252";
            this.yb_cyjsbz.InfoConnection = this.infoConnection;
            this.yb_cyjsbz.MultiSetWhere = false;
            this.yb_cyjsbz.Name = "yb_cyjsbz";
            this.yb_cyjsbz.NotificationAutoEnlist = false;
            this.yb_cyjsbz.SecExcept = null;
            this.yb_cyjsbz.SecFieldName = null;
            this.yb_cyjsbz.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_cyjsbz.SelectPaging = false;
            this.yb_cyjsbz.SelectTop = 0;
            this.yb_cyjsbz.SiteControl = false;
            this.yb_cyjsbz.SiteFieldName = null;
            this.yb_cyjsbz.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_zflb
            // 
            this.yb_zflb.CacheConnection = false;
            this.yb_zflb.CommandText = "select * from bztbd where bzcodn=\'yl\' and bzusex=1 and bzmem3 like \'%z%\'";
            this.yb_zflb.CommandTimeout = 30;
            this.yb_zflb.CommandType = System.Data.CommandType.Text;
            this.yb_zflb.DynamicTableName = false;
            this.yb_zflb.EEPAlias = null;
            this.yb_zflb.EncodingAfter = null;
            this.yb_zflb.EncodingBefore = "Windows-1252";
            this.yb_zflb.InfoConnection = this.infoConnection;
            keyItem59.KeyName = "bzcomp";
            keyItem60.KeyName = "bzcodn";
            keyItem61.KeyName = "bzkeyx";
            this.yb_zflb.KeyFields.Add(keyItem59);
            this.yb_zflb.KeyFields.Add(keyItem60);
            this.yb_zflb.KeyFields.Add(keyItem61);
            this.yb_zflb.MultiSetWhere = false;
            this.yb_zflb.Name = "yb_zflb";
            this.yb_zflb.NotificationAutoEnlist = false;
            this.yb_zflb.SecExcept = null;
            this.yb_zflb.SecFieldName = null;
            this.yb_zflb.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_zflb.SelectPaging = false;
            this.yb_zflb.SelectTop = 0;
            this.yb_zflb.SiteControl = false;
            this.yb_zflb.SiteFieldName = null;
            this.yb_zflb.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // yb_xzlx
            // 
            this.yb_xzlx.CacheConnection = false;
            this.yb_xzlx.CommandText = "select * from bztbd where bzcodn=\'AAE140\' and bzusex=1";
            this.yb_xzlx.CommandTimeout = 30;
            this.yb_xzlx.CommandType = System.Data.CommandType.Text;
            this.yb_xzlx.DynamicTableName = false;
            this.yb_xzlx.EEPAlias = null;
            this.yb_xzlx.EncodingAfter = null;
            this.yb_xzlx.EncodingBefore = "Windows-1252";
            this.yb_xzlx.InfoConnection = this.infoConnection;
            keyItem62.KeyName = "bzcomp";
            keyItem63.KeyName = "bzcodn";
            keyItem64.KeyName = "bzkeyx";
            this.yb_xzlx.KeyFields.Add(keyItem62);
            this.yb_xzlx.KeyFields.Add(keyItem63);
            this.yb_xzlx.KeyFields.Add(keyItem64);
            this.yb_xzlx.MultiSetWhere = false;
            this.yb_xzlx.Name = "yb_xzlx";
            this.yb_xzlx.NotificationAutoEnlist = false;
            this.yb_xzlx.SecExcept = null;
            this.yb_xzlx.SecFieldName = null;
            this.yb_xzlx.SecStyle = Srvtools.SecurityStyle.ssByNone;
            this.yb_xzlx.SelectPaging = false;
            this.yb_xzlx.SelectTop = 0;
            this.yb_xzlx.SiteControl = false;
            this.yb_xzlx.SiteFieldName = null;
            this.yb_xzlx.UpdatedRowSource = System.Data.UpdateRowSource.None;
            ((System.ComponentModel.ISupportInitialize)(this.cmd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoConnection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bz05d)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bzfpjm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybbz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zlfs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ybdgys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ybmzzydjdr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bztbd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bz01h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ybdgys_bz01h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symzlb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symzzd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.syzylb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sycyzd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bz01h_cwry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzybsylb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzsyrylb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sfysyjh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzkflb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tzsyscfs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zy01h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zy01h_fysc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bztbd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bz06h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_sylb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_sysslb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ssczmc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bz02d)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_gjks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dqbh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybjbzd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_yblyjg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zylx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_mtbbz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dgyszd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ywsqlx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dbrgx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zjlx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_bzmrdr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ybyljg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_sqlx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_ydrybz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_dljsbz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_cyjsbz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_zflb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yb_xzlx)).EndInit();

        }

        #endregion
    }
}
