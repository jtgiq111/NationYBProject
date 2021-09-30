//#define USE_EPPLUS
#if USE_EPPLUS
using OfficeOpenXml;
#endif
using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yb_interfaces
{
    public partial class FrmYBMLSC : InfoForm
    {
        private static readonly Dictionary<string, MLInfo> _list = new Dictionary<string, MLInfo>();

        private string _selectedCode = "";

        //1305 F001_19970101000000_C,F002_19970101000000_C
        private static Dictionary<string, string> MLLST = new Dictionary<string, string>()
        {
            {"1301","1301_西药中成药目录" },
            {"1302","1302_中药饮片目录" },
            {"1303","1303_医疗机构制剂目录" },
            {"1305","1305_医疗服务项目目录" },
            {"1306","1306_医用耗材目录" },
            {"1307","1307_疾病与诊断目录" },
            {"1308","1308_手术操作目录" },
            {"1309","1309_门诊慢特病种目录" },
            {"1310","1310_按病种付费病种目录" },
            {"1311","1311_日间手术治疗病种目录" },
            {"1313","1313_肿瘤形态学目录" },
            {"1314","1314_中医疾病目录" },
            {"1315","1315_中医证候目录" },
        };


        //编码,版本名称所在列
        private static Dictionary<string, int> XZBH = new Dictionary<string, int>
            {
                { "1301", 84 },
                { "1302", 19 },
                { "1303", 72 },
                { "1305", 16 },
                { "1306", 71 },
                { "1307", 24 },
                { "1308", 24 },
                { "1309", 12 },
                { "1310", 13 },
                { "1311", 11 },
                { "1313", 11 },
                { "1314", 14 },
                { "1315", 14 }
            };

        #region 初始化列表

        private void InitList()
        {
            if (_list.Count != 0)
            {
                return;
            }
            _list.Add("1301", new MLInfo(@"医疗目录编码,
                                            药品商品名,
                                            通用名编号,
                                            药品通用名,
                                            化学名称,
                                            别名,
                                            英文名称,
                                            注册名称,
                                            药监本位码,
                                            药品剂型,
                                            药品剂型名称,
                                            药品类别,
                                            药品类别名称,
                                            药品规格,
                                            药品规格代码,
                                            注册剂型,
                                            注册规格,
                                            注册规格代码,
                                            每次用量,
                                            使用频次,
                                            酸根盐基,
                                            国家药品编号,
                                            用法,
                                            中成药标志,
                                            生产地类别,
                                            生产地类别名称,
                                            计价单位类型,
                                            非处方药标志,
                                            非处方药标志名称,
                                            包装材质,
                                            包装材质名称,
                                            包装规格,
                                            包装数量,
                                            功能主治,
                                            给药途径,
                                            说明书,
                                            开始日期,
                                            结束日期,
                                            最小使用单位,
                                            最小销售单位,
                                            最小计量单位,
                                            最小包装数量,
                                            最小包装单位,
                                            最小制剂单位,
                                            最小包装单位名称,
                                            最小制剂单位名称,
                                            转换比,
                                            药品有效期,
                                            最小计价单位,
                                            五笔助记码,
                                            拼音助记码,
                                            分包装厂家,
                                            生产企业编号,
                                            生产企业名称,
                                            特殊限价药品标志,
                                            特殊药品标志,
                                            限制使用范围,
                                            限制使用标志,
                                            药品注册证号,
                                            药品注册证号开始日期,
                                            药品注册证号结束日期,
                                            批准文号,
                                            批准文号开始日期,
                                            批准文号结束日期,
                                            市场状态,
                                            市场状态名称,
                                            药品注册批件电子档案,
                                            药品补充申请批件电子档案,
                                            国家医保药品目录备注,
                                            基本药物标志名称,
                                            基本药物标志,
                                            增值税调整药品标志,
                                            增值税调整药品名称,
                                            上市药品目录集药品,
                                            医保谈判药品标志,
                                            医保谈判药品名称,
                                            卫健委药品编码,
                                            备注,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            版本名称,
                                            儿童用药,
                                            公司名称,
                                            仿制药一致性评价药品,
                                            经销企业,
                                            经销企业联系人,
                                            经销企业授权书电子档案,
                                            国家医保药品目录剂型,
                                            国家医保药品目录甲乙类标识", table: "ybmrdr")
            {
                Code = "1301",
                Name = "西药中成药目录",
                VerColumn = 84,
                SFLB = " case when  LEFT([医疗目录编码],1) ='X' then '101' else '102' end",
                SFLBMC = " case when  LEFT([医疗目录编码],1) ='X' then '西药' else '中成药' end"
            });

            _list.Add("1302", new MLInfo(@"医疗目录编码,
                                            单味药名称,
                                            单复方标志,
                                            质量等级,
                                            中草药年份,
                                            药用部位,
                                            安全计量,
                                            常规用法,
                                            性味,
                                            归经,
                                            品种,
                                            开始日期,
                                            结束日期,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            版本名称,
                                            药材名称,
                                            功能主治,
                                            炮制方法,
                                            功效分类,
                                            药材种来源,
                                            国家医保支付政策,
                                            省级医保支付政策,
                                            标准名称,
                                            标准页码,
                                            标准电子档案", table: "ybmrdr")
            {
                Code = "1302",
                Name = "中药饮片目录",
                VerColumn = 19,
                SFLB = "'103'",
                SFLBMC = "'中药饮片'"
            });

            _list.Add("1303", new MLInfo(@"医疗目录编码,
                                        药品商品名,
                                        别名,
                                        英文名称,
                                        剂型,
                                        剂型名称,
                                        注册剂型,
                                        成分,
                                        功能主治,
                                        性状,
                                        药品规格,
                                        药品规格代码,
                                        注册规格,
                                        注册规格代码,
                                        给药途径,
                                        贮藏,
                                        使用频次,
                                        每次用量,
                                        药品类别,
                                        药品类别名称,
                                        非处方药标志,
                                        非处方药标志名称,
                                        包装材质,
                                        包装材质名称,
                                        包装规格,
                                        说明书,
                                        包装数量,
                                        最小使用单位,
                                        最小销售单位,
                                        最小计量单位,
                                        最小包装数量,
                                        最小包装单位,
                                        最小制剂单位,
                                        最小制剂单位名称,
                                        药品有效期,
                                        最小计价单位,
                                        不良反应,
                                        注意事项,
                                        禁忌,
                                        生产企业编号,
                                        生产企业名称,
                                        生产企业地址,
                                        特殊限价药品标志,
                                        批准文号,
                                        批准文号开始日期,
                                        批准文号结束日期,
                                        药品注册证号,
                                        药品注册证号开始日期,
                                        药品注册证号结束日期,
                                        转换比,
                                        限制使用范围,
                                        最小包装单位名称,
                                        注册名称,
                                        分包装厂家,
                                        市场状态,
                                        药品注册批件电子档案,
                                        药品补充申请批件电子档案,
                                        国家医保药品目录编号,
                                        国家医保药品目录备注,
                                        增值税调整药品标志,
                                        增值税调整药品名称,
                                        上市药品目录集药品,
                                        卫健委药品编码,
                                        备注,
                                        有效标志,
                                        开始时间,
                                        结束时间,
                                        唯一记录号,
                                        数据创建时间,
                                        数据更新时间,
                                        版本号,
                                        版本名称,
                                        自制剂许可证号,
                                        儿童用药,
                                        老年患者用药,
                                        医疗机构联系人姓名,
                                        医疗机构联系人电话,
                                        自制剂许可证电子档案", table: "ybmrdr")
            {
                Code = "1303",
                Name = "医疗机构制剂目录",
                VerColumn = 72,
                SFLB = "'104'",
                SFLBMC = "'自制剂'"
            });

            _list.Add("1305", new MLInfo(@"医疗目录编码,
                                    计价单位,
                                    计价单位名称,
                                    诊疗项目说明,
                                    诊疗除外内容,
                                    诊疗项目内涵,
                                    有效标志,
                                    备注,
                                    服务项目类别,
                                    医疗服务项目名称,
                                    项目说明,
                                    开始日期,
                                    结束日期,
                                    唯一记录号,
                                    版本号,
                                    版本名称", new string[] { "F001_19970101000000_C", "F002_19970101000000_C" }, table: "ybmrdr")
            {
                Code = "1305",
                Name = "医疗服务项目目录",
                VerColumn = 16,
                SFLB = "'201'",
                SFLBMC = "'服务项目'"
            });

            _list.Add("1306", new MLInfo(@"医疗目录编码,
                                            耗材名称,
                                            医疗器械唯一标识码,
                                            医保通用名代码,
                                            医保通用名,
                                            产品型号,
                                            规格代码,
                                            规格,
                                            耗材分类,
                                            规格型号,
                                            材质代码,
                                            耗材材质,
                                            包装规格,
                                            包装数量,
                                            产品包装材质,
                                            包装单位,
                                            产品转换比,
                                            最小使用单位,
                                            生产地类别,
                                            生产地类别名称,
                                            产品标准,
                                            产品有效期,
                                            性能结构与组成,
                                            适用范围,
                                            产品使用方法,
                                            产品图片编号,
                                            产品质量标准,
                                            说明书,
                                            其他证明材料,
                                            专机专用标志,
                                            专机名称,
                                            组套名称,
                                            机套标志,
                                            限制使用标志,
                                            医保限用范围,
                                            最小销售单位,
                                            高值耗材标志,
                                            医用材料分类代码,
                                            植入材料和人体器官标志,
                                            灭菌标志,
                                            灭菌标志名称,
                                            植入或介入类标志,
                                            植入或介入类名称,
                                            一次性使用标志,
                                            一次性使用标志名称,
                                            注册备案人名称,
                                            开始日期,
                                            结束日期,
                                            医疗器械管理类别,
                                            医疗器械管理类别名称,
                                            注册备案号,
                                            注册备案产品名称,
                                            结构及组成,
                                            其他内容,
                                            批准日期,
                                            注册备案人住所,
                                            注册证有效期开始时间,
                                            注册证有效期结束时间,
                                            生产企业编号,
                                            生产企业名称,
                                            生产地址,
                                            代理人企业,
                                            代理人企业地址,
                                            生产国或地区,
                                            售后服务机构,
                                            注册或备案证电子档案,
                                            产品影像,
                                            有效标志,
                                            唯一记录号,
                                            版本号,
                                            版本名称", table: "ybmrdr")
            {
                Code = "1306",
                Name = "医用耗材目录",
                VerColumn = 71,
                SFLB = "'301'",
                SFLBMC = "'医用材料'"
            });

            _list.Add("1307", new MLInfo(@"西医疾病诊断ID,
                                            章,
                                            章代码范围,
                                            章名称,
                                            节代码范围,
                                            节名称,
                                            类目代码,
                                            类目名称,
                                            亚目代码,
                                            亚目名称,
                                            诊断代码,
                                            诊断名称,
                                            使用标记,
                                            国标版诊断代码,
                                            国标版诊断名称,
                                            临床版诊断代码,
                                            临床版诊断名称,
                                            备注,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            版本名称", table: "ybbzmrdr")
            {
                Code = "1307",
                Name = "疾病与诊断目录",
                VerColumn = 24
            });


            _list.Add("1308", new MLInfo(@"手术标准目录ID,
                                            章,
                                            章代码范围,
                                            章名称,
                                            类目代码,
                                            类目名称,
                                            亚目代码,
                                            亚目名称,
                                            细目代码,
                                            细目名称,
                                            手术操作代码,
                                            手术操作名称,
                                            使用标记,
                                            团标版手术操作代码,
                                            团标版手术操作名称,
                                            临床版手术操作代码,
                                            临床版手术操作名称,
                                            备注,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            版本名称", table: "ybbzmrdr")
            {
                Code = "1308",
                Name = "手术操作目录",
                VerColumn = 24
            });

            _list.Add("1309", new MLInfo(@"门慢门特病种目录代码,
                                        门慢门特病种大类名称,
                                        门慢门特病种细分类名称,
                                        医保区划,
                                        备注,
                                        有效标志,
                                        唯一记录号,
                                        数据创建时间,
                                        数据更新时间,
                                        版本号,
                                        病种内涵,
                                        版本名称,
                                        诊疗指南页码,
                                        诊疗指南电子档案,
                                        门慢门特病种名称,
                                        门慢门特病种大类代码", table: "ybbzmrdr")
            {
                Code = "1309",
                Name = "门诊慢特病种目录",
                VerColumn = 12
            });


            _list.Add("1310", new MLInfo(@"病种结算目录ID,
                                            按病种结算病种目录代码,
                                            按病种结算病种名称,
                                            限定手术操作代码,
                                            限定手术操作名称,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            病种内涵,
                                            备注,
                                            版本名称,
                                            诊疗指南页码,
                                            诊疗指南电子档案", table: "ybbzmrdr")
            {
                Code = "1310",
                Name = "按病种付费病种目录",
                VerColumn = 13
            });

            _list.Add("1311", new MLInfo(@"日间手术治疗目录ID,
                                    日间手术病种目录代码,
                                    日间手术病种名称,
                                    有效标志,
                                    唯一记录号,
                                    数据创建时间,
                                    数据更新时间,
                                    版本号,
                                    病种内涵,
                                    备注,
                                    版本名称,
                                    诊疗指南页码,
                                    诊疗指南电子档案,
                                    手术操作名称,
                                    手术操作代码", table: "ybbzmrdr")
            {
                Code = "1311",
                Name = "日间手术治疗病种目录",
                VerColumn = 11
            });

            _list.Add("1313", new MLInfo(@"肿瘤形态学ID,
                                            肿瘤/细胞类型代码,
                                            肿瘤/细胞类型,
                                            形态学分类代码,
                                            形态学分类,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            版本名称", table: "ybbzmrdr")
            {
                Code = "1313",
                Name = "肿瘤形态学目录",
                VerColumn = 11
            });

            _list.Add("1314", new MLInfo(@"中医疾病诊断ID,
                                            科别类目代码,
                                            科别类目名称,
                                            专科系统分类目代码,
                                            专科系统分类目名称,
                                            疾病分类代码,
                                            疾病分类名称,
                                            备注,
                                            有效标志,
                                            唯一记录号,
                                            数据创建时间,
                                            数据更新时间,
                                            版本号,
                                            版本名称", table: "ybbzmrdr")
            {
                Code = "1314",
                Name = "中医疾病目录",
                VerColumn = 14
            });


            _list.Add("1315", new MLInfo(@"中医证候ID,
                                        证候类目代码,
                                        证候类目名称,
                                        证候属性代码,
                                        证候属性,
                                        证候分类代码,
                                        证候分类名称,
                                        备注,
                                        有效标志,
                                        唯一记录号,
                                        数据创建时间,
                                        数据更新时间,
                                        版本号,
                                        版本名称", table: "ybbzmrdr")
            {
                Code = "1315",
                Name = "中医证候目录",
                VerColumn = 14
            });
        } 
        #endregion

        public FrmYBMLSC() : this("")
        {



        }

        public FrmYBMLSC(string code)
        {
            InitializeComponent();

            InitList();

            InitVer();

            if (!_list.TryGetValue(code, out var foundInfo))
            {
                return;
            }

            AddML(foundInfo);

            _selectedCode = code;
        }

        private void InitVer()
        {
            string sqlStr = string.Format(@"select code,isnull(ver, '') as ver from [DBNEWYB]..[ybvermr] where id in (select MAX(id) from [DBNEWYB]..[ybvermr] group by code)");
            DataSet dsCol = CliUtils.ExecuteSql("scomm", "cmd", sqlStr, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataRowCollection rows = dsCol.Tables[0].Rows;
            foreach (DataRow row in rows)
            {
                string code = row["code"].ToString();
                string ver = row["ver"].ToString();
                mrVar[code] = ver;
            }
        }

        private readonly Dictionary<string, string> mrVar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private void FrmYBMLSC_Load(object sender, EventArgs e)
        {
            Resize += FrmYBMLSC_Resize;

            label_file_path.Text = $"{Application.StartupPath}\\YBDOWNLOAD";

            if (!string.IsNullOrWhiteSpace(_selectedCode)) return;

            foreach (var item in _list)
            {
                AddML(item.Value);
            }

            progressBar1.Controls.Add(label_pro);
        }

        private void FrmYBMLSC_Resize(object sender, EventArgs e)
        {
            tabControl_ml.Refresh();
        }

        private void AddML(MLInfo mlInfo)
        {
            TabPage tabPage = new TabPage()
            {
                Tag = mlInfo,
                Text = $"[{mlInfo.Code}]{mlInfo.Name}"
            };


            DataTable dataTable = new DataTable();

            foreach (var field in mlInfo.Fields)
            {
                dataTable.Columns.Add(field.Key, typeof(string));
            }

            DataGridView dataGridView = new DataGridView
            {
                DataSource = dataTable,

                ReadOnly = true,
                Width = tabPage.Width,
                Height = tabPage.Height,
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToOrderColumns = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
            };

            dataGridView.DataBindingComplete += (sender, e) =>
              {

                  mlInfo.OnFinish(dataGridView);
              };


            tabPage.Controls.Add(dataGridView);

            tabControl_ml.Controls.Add(tabPage);

            mlInfo.Init(dataGridView);

            mlInfo.InitVer(mrVar);

            UpdateCurTableMaxVer();
        }

        public void SetProgress(int pro)
        {
            progressBar1.Value = pro;

            label_pro.Text = $"{pro}%";
        }

        public void SetTooltipInfo(int cur, int max)
        {
            toolTip1.SetToolTip(progressBar1, $"{cur}/{max}");
        }

        public void SetInfo(string text)
        {
            label_info.Text = text;
        }

        public void SetVer(string ver)
        {
            textBox_maxver.Text = ver;
        }

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        private static void ExplorePath(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        public void UpdateCurTableMaxVer()
        {
            if (tabControl_ml.SelectedTab == null || !(tabControl_ml.SelectedTab.Tag is MLInfo mlInfo))
            {
                textBox_maxver.Text = "";
                return;
            }

            textBox_maxver.Text = mlInfo.CurMaxVer;
        }

        //导出到excel
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!(tabControl_ml.SelectedTab.Tag is MLInfo miInfo)) return;

            miInfo.Reset();
        }

        //同步到ybmrdr/ybbzmrdr
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (!(tabControl_ml.SelectedTab.Tag is MLInfo miInfo)) return;

            if (!int.TryParse(textBox_repCount.Text, out int count))
            {
                count = 2000;
            }

            miInfo.Update(this, count);
        }

        //download
        private void button_download_Click(object sender, EventArgs e)
        {
            if (!(tabControl_ml.SelectedTab.Tag is MLInfo miInfo)) return;

            if (!(tabControl_ml.SelectedTab.Controls[0] is DataGridView dataGridView)) return;

            miInfo.CurMaxVer = textBox_maxver.Text;
            mrVar[miInfo.Code] = textBox_maxver.Text;

            if (!int.TryParse(textBox_bulk.Text, out int count))
            {
                count = 30;
            }

            if (!int.TryParse(textBox_repCount.Text, out int pcount))
            {
                pcount = 2000;
            }


            miInfo.DownloadAsync(dataGridView, this, count, miInfo.CurMaxVer, pcount);
        }

        private void tabControl_ml_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = MLInfo.Downloading;
        }

        internal void BeginDownload()
        {
            textBox_maxver.Enabled = false;
            button_download.Enabled = false;
            button4.Enabled = false;
        }

        internal void EndDownload()
        {
            textBox_maxver.Enabled = true;
            button_download.Enabled = true;
            button4.Enabled = true;
        }

        private void label_file_path_Click(object sender, EventArgs e)
        {
            ExplorePath(label_file_path.Text);
        }

        private void tabControl_ml_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurTableMaxVer();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    class MLInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string SFLB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SFLBMC { get; set; }

        public List<string> TSFLB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 接口编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 版本名称所在列(索引从1开始的)
        /// </summary>
        public int VerColumn { get; set; }

        /// <summary>
        /// 字段列表
        /// </summary>
        public Dictionary<string, string> Fields { get; set; }

        /// <summary>
        /// 初始版本号
        /// </summary>
        public string[] InitVarList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HashSet<string> Vers { get; set; }

        /// <summary>
        /// 当前版本号
        /// </summary>
        public string CurMaxVer { get; set; }

        public MLInfo(string fieldStr, string[] initVar = null, string table = null)
        {
            Fields = new Dictionary<string, string>();

            Vers = new HashSet<string>();

            TSFLB = new List<string>();

            if (initVar == null)
            {
                InitVarList = new string[] { "0" };
            }

            if (table == null)
            {
                table = "ybmrdr";
            }

            Table = table;

            var names = fieldStr.Split(',');

            foreach (var item in names)
            {
                Fields.Add(item.Trim().Replace("\r\n", ""), "");
            }
        }

        public virtual void InitVer(Dictionary<string, string> mrVar)
        {
            if (!mrVar.TryGetValue(Code, out var curMaxVar))
            {
                if (Code == "1305")
                {
                    curMaxVar = "F002_19970101000000_C";
                }
                else
                {
                    curMaxVar = "0";
                }
                mrVar[Code] = curMaxVar;
            }

            CurMaxVer = curMaxVar;
        }

        public virtual void Init(DataGridView dataGridView)
        {

            string strSql = string.Format(@" select columnName,tabColumn from colCompare where tableName='{0}' and beLong='{1}' order by columnOrder ", Table, Code);
            DataSet dsCol = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataRowCollection rows = dsCol.Tables[0].Rows;

            if (rows.Count != Fields.Count)
            {
                MessageBox.Show($"{Code}_{Name} 字段映射不一致");
                return;
            }
            foreach (DataRow row in rows)
            {
                string columnName = row["columnName"].ToString();
                string tabColumn = row["tabColumn"].ToString();

                if (Fields.ContainsKey(columnName))
                {
                    Fields[columnName] = tabColumn;
                }
                else
                {
                    MessageBox.Show($"{Code}|{Name}|{columnName}对应字段为空{tabColumn}");
                }
            }
        }

        public virtual void OnFinish(DataGridView dataGridView)
        {
            if (!(dataGridView.DataSource is DataTable dataTable)) return;

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                DataGridViewColumn column = dataGridView.Columns[i];

                column.SortMode = DataGridViewColumnSortMode.NotSortable;

                if (i == 0)
                {
                    column.HeaderCell.Style.ForeColor = Color.Red;
                    column.Width = 240;
                }
                else if (i == VerColumn - 1)
                {
                    column.HeaderCell.Style.ForeColor = Color.Red;
                    column.DefaultCellStyle.ForeColor = Color.Red;

                    column.Width = 210;
                }
            }
        }

        public virtual void Export(DataGridView dataGridView)
        {
            if (!(dataGridView.DataSource is DataTable dataTable)) return;

            int rowNumber = dataTable.Rows.Count;//不包括字段名

            if (rowNumber == 0)
            {
                MessageBox.Show("无数据");
                return;
            }

            SaveFileDialog file = new SaveFileDialog();//定义新的文件保存位置控件
            file.FileName = $"{Code}_{Name}";
            file.Filter = "xls文件(*.xlsx)|*.xlsx";//设置文件后缀的过滤
            if (file.ShowDialog() != DialogResult.OK)//如果有文件保存路径
            {
                return;
            }

#if USE_EPPLUS
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(file.FileName)))
            {
                var worksheet = package.Workbook.Worksheets.Add(Name);

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var col = dataTable.Columns[i];

                    worksheet.Cells[1, i + 1].Value = col.ColumnName;
                    worksheet.Column(i+1).Width = 25;
                }

                int startRow = 2;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];

                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        worksheet.Cells[startRow + i, j + 1].Value = row[j].ToString();
                    }
                }

                package.Save();

                MessageBox.Show("导出成功!");
            }
#endif 
        }

        /// <summary>
        /// 1.下载数据
        /// </summary>
        /// <param name="frmYBMLSC"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool Download(FrmYBMLSC frmYBMLSC, DataTable dataTable, string ver)
        {
            try
            {
                if (Code == "1305")
                {
                    if (ver == "0" || ver == "1")
                    {
                        ver = "F002_19970101000000_C";
                    }
                }

                CurMaxVer = ver;

                string db = "DBNEWYB";
                string tableName = $"{Code}_{Name}";

                bool lastVer = false;
                string err = "";
                while (true)
                {
                    frmYBMLSC.BeginInvoke(new Action(() =>
                    {
                        frmYBMLSC.SetProgress(0);
                        frmYBMLSC.SetInfo($"{tableName} 1.开始下载(版本号{CurMaxVer})...");
                    }));

                    //yb_interface_jsxybxh_dr.WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {CliUtils.fLoginUser} 开始下载|{Code}|{Name}|{CurMaxVer}");

                    object[] objRet = yb_interface_jx.YBMLXZ(new object[] { Code, CurMaxVer });

                    if (objRet[1].ToString() == "1")
                    {
                        //文件路径
                        string path = objRet[2].ToString();
                        string oldVer = CurMaxVer;

                        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = new StreamReader(fs))
                            {
                                string nextVer = null;
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine()?.Split('\t');
                                    if (line == null || line.Length == 0)
                                    {
                                        continue;
                                    }

                                    var row = dataTable.NewRow();

                                    for (int i = 0; i < dataTable.Columns.Count; i++)
                                    {
                                        string content = line[i].Replace("'", "''");
                                        row[i] = content;

                                        if (nextVer == null && i == VerColumn - 1)
                                        {
                                            nextVer = line[i];
                                        }
                                    }
                                    dataTable.Rows.Add(row);
                                }

                                if (string.IsNullOrEmpty(nextVer))
                                {
                                    break;
                                }
                                if (0 == string.Compare(nextVer, CurMaxVer, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    break;
                                }
                                CurMaxVer = nextVer;
                                //更新最新版本
                                string verTableName = $"[{db}]..[ybvermr]";
                                string strSql = string.Format(@"INSERT INTO {0} (code,name,ver,sysdate) VALUES ('{1}','{2}','{3}','{4}') ", verTableName, Code, Name, CurMaxVer, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                DataSet dataSet = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                                //yb_interface_jsxybxh_dr.WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {CliUtils.fLoginUser} 当前版本号|{Code}|{Name}|{CurMaxVer}");
                            }
                        }

                        frmYBMLSC.BeginInvoke(new Action(() =>
                        {
                            frmYBMLSC.SetProgress(100);
                            frmYBMLSC.SetInfo($"{tableName} 1.下载完毕(版本号{oldVer}),下一个版本{CurMaxVer}...");
                            frmYBMLSC.SetVer(CurMaxVer);
                        }));
                    }
                    else
                    {
                        err = objRet[2].ToString();
                        //暂时这么处理
                        if (err.Contains("FSI-已经是最新数据"))
                        {
                            //下载到最新版本了
                            lastVer = true;
                        }
                        break;
                    }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!lastVer)
                {
                    frmYBMLSC.BeginInvoke(new Action(() =>
                    {
                        frmYBMLSC.SetProgress(100);
                        frmYBMLSC.SetInfo($"{tableName} 1.下载错误(版本号{CurMaxVer}){err}...");
                    }));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 2.显示到网格
        /// </summary>
        /// <param name="frmYBMLSC"></param>
        /// <param name="dataGridView"></param>
        /// <param name="dataTable"></param>
        public void ReflushDataGrid(FrmYBMLSC frmYBMLSC, DataGridView dataGridView, DataTable dataTable)
        {
            string tableName = $"{Code}_{Name}";

            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetProgress(0);
                frmYBMLSC.SetInfo($"{tableName} 2.加载中...");
            }));

            dataGridView.BeginInvoke(new Action(() =>
            {
                dataGridView.DataSource = dataTable;
            }));

            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetProgress(0);
                frmYBMLSC.SetInfo($"{tableName} 2.加载完毕...");
            }));
        }

        /// <summary>
        /// 3.同步到临时表
        /// </summary>
        /// <param name="frmYBMLSC"></param>
        /// <param name="dataGridView"></param>
        /// <param name="dataTable"></param>
        /// <param name="updateToYBMRDR"></param>
        public void UploadToTempTable(FrmYBMLSC frmYBMLSC, DataGridView dataGridView, DataTable dataTable, bool updateToYBMRDR, int bulkCount = 50,int perCount = 2000)
        {
            try
            {
                //当前insert的数量(每次达到最大值(bulkCount)都会重置)
                int insertSqlCount = 0;
                //总的insert的数量
                int insertedCount = 0;

                string db = "DBNEWYB";
                string tableName = $"{Code}_{Name}";

                dataGridView.BeginInvoke(new Action(() =>
                {
                    frmYBMLSC.SetInfo($"{tableName} 3.开始上传到临时表..");
                    frmYBMLSC.SetProgress(0);
                }));

                //删除临时表中当前需要更新的版本
                string deleteSql = $"truncate table [{db}]..[{tableName}]";
                CliUtils.ExecuteSql("scomm", "cmd", deleteSql, true, CliUtils.fCurrentProject);

                //开始更新到临时表
                string insertHeader = GetInsertHeader(dataTable, db, tableName);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(insertHeader);

                for (int i = 0; i < dataTable.Rows.Count; ++i)
                {
                    DataRow row = dataTable.Rows[i];

                    if (insertSqlCount >= bulkCount)
                    {
                        insertSqlCount = 0;

                        string sqlSb = sb.ToString();

                        try
                        {

                            CliUtils.ExecuteSql("scomm", "cmd", sqlSb, true, CliUtils.fCurrentProject);
                        }
                        catch (Exception ex)
                        {
                            string err = $"{ex.Message}+\r\n+{ex.StackTrace}+\r\n+{sqlSb}";
                            //yb_interface_jsxybxh_dr.WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {CliUtils.fLoginUser} {Code}|{Name}|{sqlSb}");

                            frmYBMLSC.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show(err);
                            }));
                        }

                        sb = new StringBuilder();

                        sb.AppendLine(insertHeader);

                        dataGridView.BeginInvoke(new Action(() =>
                        {
                            frmYBMLSC.SetProgress((int)((float)insertedCount / dataTable.Rows.Count * 100));
                        }));
                    }
                    sb.Append(GetInsertRow(dataTable, row));
                    if (insertedCount < dataTable.Rows.Count - 1 && insertSqlCount < bulkCount - 1)
                    {
                        sb.AppendLine(",");
                    }
                    insertSqlCount++;

                    insertedCount++;
                }

                string sqlSb0 = sb.ToString();
                if (insertSqlCount != 0)
                {
                    try
                    {
                        CliUtils.ExecuteSql("scomm", "cmd", sqlSb0, true, CliUtils.fCurrentProject);
                    }
                    catch (Exception ex)
                    {
                        string err = $"{ex.Message}+\r\n+{ex.StackTrace}+\r\n+{sqlSb0}";

                        //yb_interface_jsxybxh_dr.WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {CliUtils.fLoginUser} {Code}|{Name}|{sqlSb0}");

                        frmYBMLSC.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(err);
                        }));
                    }

                    dataGridView.BeginInvoke(new Action(() =>
                    {
                        frmYBMLSC.SetProgress((int)((float)insertedCount / dataTable.Rows.Count * 100));
                    }));
                }

                if (updateToYBMRDR)
                {
                    UpdateToYbmrdr(frmYBMLSC,perCount);
                }

                //更新完毕
                dataGridView.BeginInvoke(new Action(() =>
                {
                    frmYBMLSC.SetInfo($"{tableName} 3.已更新完成");
                }));
            }
            catch (Exception ex)
            {
                string err = $"{ex.Message}+\r\n+{ex.StackTrace}";

                //yb_interface_jsxybxh_dr.WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {CliUtils.fLoginUser} {Code}|{Name}|{err}");

                frmYBMLSC.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(err);
                }));
            }
        }

        public void UpdateToYbmrdr(FrmYBMLSC frmYBMLSC, int perCount = 2000)
        {
            if(perCount <= 0)
            {
                MessageBox.Show("同步条数不能小于1");
                return;
            }

            string db = "DBNEWYB";
            string tableName = $"{Code}_{Name}";

            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetInfo($"{tableName} 4.准备同步到{Table}...");
            }));

            //
            string sqlStr = $"select [版本名称] as bbmc,COUNT(*) as Cnt from [{db}]..[{tableName}] group by[版本名称]";
            DataSet ds = CliUtils.ExecuteSql("scomm", "cmd", sqlStr, true, CliUtils.fCurrentProject);

            int tCount = 0;
            HashSet<string> verList = new HashSet<string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string bbmc = row["bbmc"].ToString();
                string cnt = row["Cnt"].ToString();
                verList.Add(bbmc);
                tCount += Convert.ToInt32(cnt);
            }

            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetInfo($"{tableName} 4.获取到数据总数{tCount}...");
            }));

            int exCount = tCount / perCount;
            if (tCount % perCount != 0)
            {
                exCount++;
            }

            //删除已经存在的
            StringBuilder sb = new StringBuilder();


            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetInfo($"{tableName} 4.开始删除已经存在的数据...");
            }));

            //int td = 0;
            //foreach (var item in verList)
            //{
            //    string sql = "";
            //    if(Code == "1301")
            //    {
            //        sql = $"delete [{Table}] where sflbdm in ('101','102') and bbmc = '{item}';";
            //    }
            //    else
            //    {
            //        sql = $"delete [{Table}] where sflbdm = {SFLB} and bbmc = '{item}';";
            //    }

            //    CliUtils.ExecuteSql("scomm", "cmd", sql, true, CliUtils.fCurrentProject);

            //    frmYBMLSC.BeginInvoke(new Action(() =>
            //    {
            //        frmYBMLSC.SetInfo($"{tableName} 4.删除中({td++}/{verList.Count})...");
            //    }));
            //}

            //frmYBMLSC.BeginInvoke(new Action(() =>
            //{
            //    frmYBMLSC.SetInfo($"{tableName} 4.删除已经存在的数据完成...");
            //}));

            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetProgress(0);

                frmYBMLSC.SetInfo($"{tableName} 4.开始同步到{Table}...");
            }));


            for (int i = 0; i < exCount; i++)
            {
                //临时表更新完毕 开始更新到ybmrdr
                string insert = GetUpdateTOYBMRDRSql(Table, db, tableName, i * perCount, i * perCount + perCount);

                if (!string.IsNullOrWhiteSpace(insert))
                {
                    CliUtils.ExecuteSql("scomm", "cmd", insert, true, CliUtils.fCurrentProject);

                    frmYBMLSC.BeginInvoke(new Action(() =>
                    {
                        frmYBMLSC.SetProgress((int)((float)i / exCount * 100));

                        frmYBMLSC.SetInfo($"{tableName} 4.同步到{Table}({i}/{exCount})...");
                    }));
                }
            }

            frmYBMLSC.BeginInvoke(new Action(() =>
            {
                frmYBMLSC.SetProgress(100);

                frmYBMLSC.SetInfo($"{tableName} 4.同步完成{Table}...");
            }));
            MessageBox.Show("已完成");
        }

        private static Task _downloadingTask = null;

        /// <summary>
        /// 
        /// </summary>
        public static bool Downloading = _downloadingTask != null && !_downloadingTask.IsCompleted;

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="frmYBMLSC"></param>
        public void DownloadAsync(DataGridView dataGridView, FrmYBMLSC frmYBMLSC, int count, string ver, int pCount)
        {
            if (_downloadingTask != null && !_downloadingTask.IsCompleted)
            {
                MessageBox.Show("任务执行中");
                return;
            }

            if (DialogResult.Yes != MessageBox.Show($"是否开始下载{Code}_{Name}(当前版本号{CurMaxVer})?可能需要非常长的时间!", "", MessageBoxButtons.YesNo))
            {
                return;
            }

            if (!(dataGridView.DataSource is DataTable srcDataTable)) return;

            var dataTable = srcDataTable.Clone();

            _downloadingTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    frmYBMLSC.BeginInvoke(new Action(() =>
                    {
                        frmYBMLSC.BeginDownload();
                    }));

                    if (!Download(frmYBMLSC, dataTable, ver))
                    {
                        frmYBMLSC.BeginInvoke(new Action(() =>
                        {
                            frmYBMLSC.EndDownload();
                        }));
                        return;
                    }

                    ReflushDataGrid(frmYBMLSC, dataGridView, dataTable);

                    UploadToTempTable(frmYBMLSC, dataGridView, dataTable, false, count, pCount);

                    frmYBMLSC.BeginInvoke(new Action(() =>
                    {
                        frmYBMLSC.EndDownload();
                    }));
                }
                catch (Exception)
                {

                }
            });
        }

        private Task _uploadTask;

        public void Update(FrmYBMLSC frmYBMLSC,int count)
        {
            if (_uploadTask != null && !_uploadTask.IsCompleted)
            {
                MessageBox.Show("任务进行中");
                return;
            }
            _uploadTask = Task.Factory.StartNew(() =>
           {
               UpdateToYbmrdr(frmYBMLSC,count);
           });
        }

        private string GetInsertHeader_ybmrdr(string tableName)
        {

            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.Append("[sflbdm],[sflb],");
            foreach (var dc in Fields)
            {
                sb.Append($"[{dc.Value}]");
                if (count++ < Fields.Count - 1)
                {
                    sb.Append(",");
                }
            }
            //string tt = $@"with temp as
            //            (
            //            select distinct bbmc from ybmrdr where sflbdm = {SFLB}
            //            )";

            //{tt}\r\n
            return $@"SET NOCOUNT ON
                        BEGIN TRANSACTION
                        Insert into [{tableName}] ({ sb}) ";
        }

        private string GetUpdateTOYBMRDRSql(string ybmrdr, string db, string tableName, int min, int max)
        {
            //select [西医疾病诊断ID],[诊断名称],[版本名称] from DBNewYB..[1307_疾病与诊断目录] 
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var dc in Fields)
            {
                sb.Append($"[{dc.Key}]");
                if (count++ < Fields.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return $@"{GetInsertHeader_ybmrdr(ybmrdr)} 
                        select {SFLB} as sflbdm,{SFLBMC} as sflb, {sb} from [{db}]..[{tableName}]
                        where id >= {min} and id < {max}
                        COMMIT";
                //$" and [版本名称] not in (select bbmc from temp)";
        }

        private string GetInsertHeader(DataTable dt, string db, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                if (0 == string.Compare(dc.ColumnName, "id", StringComparison.CurrentCultureIgnoreCase))
                {
                    count++;
                    continue;
                }
                sb.Append($"[{dc.ColumnName}]");
                if (count++ < dt.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return $"Insert into [{db}]..[{tableName}] ({ sb}) VALUES";
        }

        private string GetInsertRow(DataTable dt, DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                sb.Append($"N'{row[dc.ColumnName]}'");
                if (count++ < dt.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return $" ({sb})";
        }

        public void Reset()
        {
            if (DialogResult.Yes != MessageBox.Show("是否清空历史下载记录?", "", MessageBoxButtons.YesNo))
            {
                return;
            }
            string db = "DBNEWYB";
            string verTableName = $"[{db}]..[ybvermr]";
            string strSql = string.Format(@"delete {0} where code = '{1}' ", verTableName, Code);

            CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            MessageBox.Show("完成!");
        }
    }
}
