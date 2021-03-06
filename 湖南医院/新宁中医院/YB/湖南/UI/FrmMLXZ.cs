using Newtonsoft.Json.Linq;
using Srvtools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using yb_interfaces.YB.湖南.Enum;

namespace yb_interfaces
{
    public partial class FrmMLXZ : InfoForm
    {
        public FrmMLXZ()
        {
            InitializeComponent();
            cbxXZZL.SelectedIndex = 0;
        }

        #region Excel转换类
        /// <summary>
        /// 工作簿
        /// </summary>
        [XmlRoot(ElementName = "Workbook")]
        public class Workbook : INotifyPropertyChanged
        {
            internal const string SPREADSHEETSTRING = "urn:schemas-microsoft-com:office:spreadsheet";
            internal const string OFFICESTRING = "urn:schemas-microsoft-com:office:office";
            internal const string EXCELSTRING = "urn:schemas-microsoft-com:office:excel";



            [XmlAttribute]
            public string xmlns = "urn:schemas-microsoft-com:office:spreadsheet";


            ObservableCollection<Style> styles = new ObservableCollection<Style>();
            /// <summary>
            /// 样式集合（样式必须添加到工作簿才能有效）
            /// </summary>
            [XmlArray(ElementName = "Styles")]
            public ObservableCollection<Style> Styles
            {
                get { return styles; }
                set
                {
                    if (value != styles)
                    {
                        styles = value;
                        NotifyPropertyChanged("Styles");
                    }
                }
            }

            ObservableCollection<Worksheet> sheets = new ObservableCollection<Worksheet>();
            /// <summary>
            /// 工作表集合
            /// </summary>
            [XmlElement(ElementName = "Worksheet")]
            public ObservableCollection<Worksheet> Sheets
            {
                get { return sheets; }
                set
                {
                    if (value != sheets)
                    {
                        sheets = value;
                        NotifyPropertyChanged("Sheets");
                    }
                }
            }


            public Workbook()
            {
                //if (styles.Count == 0)
                //{
                //    Style styleDefault = new Style("Default");

                //    Styles.Add(styleDefault);
                //}
            }
            /// <summary>
            /// 追加工作表
            /// </summary>
            /// <param name="sheetName"></param>
            /// <returns></returns>
            public Worksheet AppendSheet(string sheetName)
            {
                Worksheet worksheet = new Worksheet(sheetName, this);
                return worksheet;

            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            /// <summary>
            /// 保存至Excel文件
            /// </summary>
            /// <param name="filePath">文件路径</param>
            public void Save(string filePath)
            {

                for (int i = 0; i < Sheets.Count; i++)
                {
                    if (Sheets[i].Name == "")
                    {
                        Sheets[i].Name = "Sheet" + i.ToString();
                    }
                }
                foreach (Worksheet sheet in Sheets)
                {
                    //if (sheet.Table.Columns != null && sheet.Table.Columns.Count > 0)
                    //{
                    //    var res = sheet.Table.Columns.OrderBy(x => x.Index).ToArray();
                    //    sheet.Table.Columns.Clear();
                    //    for (int i = 0; i < res.Length; i++)
                    //    {
                    //        sheet.Table.Columns.Add(res[i]);
                    //    }
                    //}
                }

                FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.NamespaceHandling = NamespaceHandling.Default;

                settings.Indent = true;
                settings.NewLineChars = Environment.NewLine;
                XmlWriter writer = XmlWriter.Create(stream, settings);

                writer.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");


                #region 给带命名空间的属性自定义序列化
                //XmlAttributes attributes = new XmlAttributes();
                //attributes.XmlDefaultValue = 0;
                //attributes.XmlAttribute = new XmlAttributeAttribute("Index");
                //XmlAttributeOverrides overrides = new XmlAttributeOverrides();

                //overrides.Add(typeof(Row),"Index", attributes);



                //SetDefaultValue(typeof(Row), "Index", 0);
                //SetDefaultValue(typeof(Row), "Height", 18);
                //SetDefaultValue(typeof(Cell), "MergeAcross", 0);
                //SetDefaultValue(typeof(Cell), "MergeDown", 0);
                #endregion




                XmlSerializer serializer = new XmlSerializer(typeof(Workbook));

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", SPREADSHEETSTRING);
                namespaces.Add("o", OFFICESTRING);
                namespaces.Add("x", EXCELSTRING);
                namespaces.Add("ss", SPREADSHEETSTRING);
                namespaces.Add("html", "http://www.w3.org/TR/REC-html40");

                serializer.Serialize(writer, this, namespaces);
                stream.Close();

                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode node = doc.SelectSingleNode("//Workbook");
                XmlAttribute att = doc.CreateAttribute("xmlns");
                att.Value = SPREADSHEETSTRING;
                node.Attributes.Append(att);

                XmlNodeList nodeWorksheetOptionsList = doc.SelectNodes("//WorksheetOptions");
                foreach (XmlNode nodeWorksheetOptions in nodeWorksheetOptionsList)
                {
                    XmlAttribute attWorksheetOptions = doc.CreateAttribute("xmlns");
                    attWorksheetOptions.Value = EXCELSTRING;
                    nodeWorksheetOptions.Attributes.Append(attWorksheetOptions);
                }
                XmlNodeList nodeDataVaildations = doc.SelectNodes("//DataValidation");
                foreach (XmlNode nodeDataVaildation in nodeDataVaildations)
                {
                    XmlAttribute attDataVaildation = doc.CreateAttribute("xmlns");
                    attDataVaildation.Value = EXCELSTRING;
                    nodeDataVaildation.Attributes.Append(attDataVaildation);
                }


                XmlNodeList nodeAutoFilter = doc.SelectNodes("//AutoFilter");
                foreach (XmlNode node1 in nodeAutoFilter)
                {
                    XmlAttribute att1 = doc.CreateAttribute("xmlns");
                    att1.Value = EXCELSTRING;
                    node1.Attributes.Append(att1);
                }

                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("ss", SPREADSHEETSTRING);
                XmlNodeList nodeListIndex = doc.SelectNodes("//*[@ss:Index='0']", manager);
                if (nodeListIndex.Count > 0)
                {
                    foreach (XmlNode n in nodeListIndex)
                    {
                        n.Attributes.RemoveNamedItem("Index", SPREADSHEETSTRING);
                    }
                }

                doc.Save(filePath);

            }
            /// <summary>
            /// 加载Excel表文件（只支持xml格式的文件，不支持二进制文件）
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public static Workbook Load(string filePath)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Workbook));

                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);
                string str = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                str = str.Replace("xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"", "");
                str = str.Replace("xmlns=\"urn:schemas-microsoft-com:office:excel\"", "");
                TextReader r = new StringReader(str);

                object obj = serializer.Deserialize(r);
                r.Close();
                return (Workbook)obj;
            }
            ///// <summary>
            ///// 给带命名空间的属性自定义序列化（给其一默认值，如果是默认值则不序列化）
            ///// </summary>
            ///// <param name="elementType"></param>
            ///// <param name="AttributeName"></param>
            ///// <param name="defaultValue"></param>
            //void SetDefaultValue(Type elementType,string AttributeName,object defaultValue)
            //{
            //    XmlAttributes attributes = new XmlAttributes();
            //    attributes.XmlDefaultValue = defaultValue;
            //    attributes.Xmlns = true;
            //    attributes.XmlAttribute = new XmlAttributeAttribute(AttributeName);
            //    attributes.XmlAttribute.Namespace = SPREADSHEETSTRING;
            //    overrides.Add(elementType, AttributeName, attributes);
            //}

            public Style CreateStyle(string styleID = "")
            {
                Style style = new Style(styleID);
                Styles.Add(style);
                return style;
            }
        }
        /// <summary>
        /// 工作表
        /// </summary>
        public class Worksheet : INotifyPropertyChanged
        {
            ObservableCollection<NamedRange> names;
            /// <summary>
            /// 预设区域集合
            /// </summary>
            public ObservableCollection<NamedRange> Names
            {
                get { return names; }
                set
                {
                    if (value != names)
                    {
                        names = value;
                        NotifyPropertyChanged("Names");
                    }
                }
            }
            string name = "";
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = Workbook.SPREADSHEETSTRING)]
            public string Name
            {
                get { return name; }
                set
                {
                    if (value != name)
                    {
                        name = value;
                        NotifyPropertyChanged("Name");
                    }
                }
            }
            internal Workbook workbook;
            Table table = new Table();
            /// <summary>
            /// 表
            /// </summary>
            public Table Table
            {
                get { return table; }
                set { if (value != table) { table = value; NotifyPropertyChanged("Table"); } }
            }
            public Worksheet(Workbook workbook)
            {
                this.workbook = workbook;
                workbook.Sheets.Add(this);
            }

            public Worksheet()
            {

            }
            public Worksheet(string Name, Workbook workbook)
            {
                this.workbook = workbook;
                workbook.Sheets.Add(this);
                this.Name = Name;
            }
            /// <summary>
            /// 设置可打印的区域（只会打印该区域）
            /// </summary>
            /// <param name="ranges">打印区域</param>
            public void SetPrintRange(List<Range> ranges)
            {
                if (Names == null)
                {
                    Names = new ObservableCollection<NamedRange>();
                }
                var res = from NamedRange r in Names where r.Name == "Print_Area" select r;
                NamedRange range = null;
                if (res.Count() > 0)
                {
                    range = res.First();
                }
                else
                {
                    range = new NamedRange("Print_Area");
                    Names.Add(range);
                }
                range.RefersTo = "=" + string.Join(",", ranges);
            }
            /// <summary>
            /// 设置重复打印的标题区域（每页都会重复显示的标题区域）
            /// </summary>
            /// <param name="ranges">标题所在的区域</param>
            public void SetPrintTitleRange(List<Range> ranges)
            {
                if (Names == null)
                {
                    Names = new ObservableCollection<NamedRange>();
                }
                var res = from NamedRange r in Names where r.Name == "Print_Titles" select r;
                NamedRange range = null;
                if (res.Count() > 0)
                {
                    range = res.First();
                }
                else
                {
                    range = new NamedRange("Print_Titles");
                    Names.Add(range);
                }
                range.RefersTo = "=" + string.Join(",", ranges);
            }
            /// <summary>
            /// 在指定的位置插入行
            /// </summary>
            /// <param name="index">行号</param>
            /// <returns></returns>
            public Row InsertRow(int index)
            {
                Row row = new Row();
                if (index > Table.Rows.Count - 1)
                {
                    Table.Rows.Add(row);
                }
                else
                    Table.Rows.Insert(index, row);
                return row;
            }
            /// <summary>
            /// 追加行
            /// </summary>
            /// <returns></returns>
            public Row AppendRow(double height = 18)
            {
                Row row = new Row();
                row.Height = height;
                Table.Rows.Add(row);
                return row;
            }

            /// <summary>
            /// 追加列
            /// </summary>
            /// <param name="width">列宽</param>
            /// <returns></returns>
            public Column AppendColumn(double width)
            {
                Column column = new Column();
                column.Width = width;
                if (Table.Columns == null)
                {
                    Table.Columns = new ObservableCollection<Column>();
                }
                Table.Columns.Add(column);
                return column;
            }
            /// <summary>
            /// 追加列
            /// </summary>
            /// <param name="width">列宽</param>
            /// <returns></returns>
            public Column AppendColumn(int idx, double width)
            {
                Column column = new Column();
                column.Index = idx;
                column.Width = width;
                if (Table.Columns == null)
                {
                    Table.Columns = new ObservableCollection<Column>();
                }
                Table.Columns.Add(column);
                return column;
            }
            DataTable dataSource;
            /// <summary>
            /// 工作表自动填充的数据源
            /// </summary>
            [XmlIgnore]
            public DataTable DataSource
            {
                get { return dataSource; }
                set
                {
                    if (value != dataSource)
                    {
                        dataSource = value;
                        NotifyPropertyChanged("DataSource");
                    }

                }
            }
            /// <summary>
            /// 根据数据源添加数据到工作表
            /// </summary>
            /// <param name="styleContent">内容样式</param>
            /// <param name="styleHeader">表标题样式</param>
            public void AppendData(Style styleContent = null, Style styleHeader = null, bool addTotal = false, DataTable dataTable = null)
            {
                if (dataTable == null)
                {
                    dataTable = DataSource;

                }
                if (dataTable == null)
                    return;
                Row rowHeader = AppendRow(32);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    Cell cell = new Cell(dataTable.Columns[i].Caption == "" ? dataTable.Columns[i].ColumnName : dataTable.Columns[i].Caption, i + 1, styleHeader);

                    rowHeader.Cells.Add(cell);
                }
                Style styleInt = null;
                if (styleContent != null)
                {
                    styleInt = styleContent.Copy();

                    styleInt.NumberFormat = new NumberFormat();
                    workbook.Styles.Add(styleInt);
                }

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (BeforeRowDataAdded != null)
                    {
                        BeforeRowDataAdded(this, new RowDataEventArgs() { Row = dataTable.Rows[i], RowIndex = i, CurrStyle = styleContent });
                    }
                    Row row = new Row();
                    Table.Rows.Add(row);


                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        Type type = dataTable.Rows[i][j].GetType();
                        if (type == typeof(int))
                        {
                            Cell cell = new Cell(dataTable.Rows[i][j], j + 1, styleInt);
                            row.Cells.Add(cell);
                        }
                        else
                        {
                            Cell cell = new Cell(dataTable.Rows[i][j], j + 1, styleContent);
                            row.Cells.Add(cell);
                        }


                    }
                    if (AfterRowDataAdded != null)
                    {
                        AfterRowDataAdded(this, new RowDataEventArgs() { Row = dataTable.Rows[i], RowIndex = i, CurrRow = row, CurrStyle = styleContent });
                    }
                }
                if (addTotal == true)
                {
                    Row row = AppendRow();
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (j == 0)
                        {
                            row.AddCell("合计：", 0, styleContent);
                        }
                        else
                        {
                            Type type = dataTable.Columns[j].DataType;
                            if (type == typeof(int))
                            {
                                string columName = dataTable.Columns[j].ColumnName;
                                int sum = (int)dataTable.Compute(string.Format("sum({0})", columName), "");

                                row.AddCell(sum, 0, styleInt);
                            }
                            else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
                            {
                                string columName = dataTable.Columns[j].ColumnName;
                                object sum = dataTable.Compute(string.Format("sum({0})", columName), "");
                                row.AddCell(sum, 0, styleContent);
                            }
                        }

                    }
                }


            }

            public delegate void BeforeRowDataAddedDelegate(object sender, RowDataEventArgs e);
            public event BeforeRowDataAddedDelegate BeforeRowDataAdded;
            public delegate void AfterRowDataAddedDelegate(object sender, RowDataEventArgs e);
            public event AfterRowDataAddedDelegate AfterRowDataAdded;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            WorksheetOptions worksheetOptions = new WorksheetOptions();
            /// <summary>
            /// 工作表设置选项
            /// </summary>
            public WorksheetOptions WorksheetOptions
            {
                get { return worksheetOptions; }
                set
                {
                    if (value != worksheetOptions)
                    {
                        worksheetOptions = value;
                        NotifyPropertyChanged("WorksheetOptions");
                    }
                }
            }
            AutoFilter autoFilter;
            /// <summary>
            /// 自动过滤设置
            /// </summary>
            public AutoFilter AutoFilter
            {
                get { return autoFilter; }
                set
                {
                    if (value != autoFilter)
                    {
                        autoFilter = value;
                        NotifyPropertyChanged("AutoFiter");
                    }
                }
            }
            /// <summary>
            /// 复制工作表
            /// </summary>
            /// <returns></returns>
            public Worksheet Copy()
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Worksheet));
                MemoryStream stream = new MemoryStream();
                xmlSerializer.Serialize(stream, this);
                stream.Position = 0;
                Worksheet sheet = (Worksheet)xmlSerializer.Deserialize(stream);
                sheet.Name = Name + "_副本";
                stream.Close();
                return sheet;
            }
            /// <summary>
            /// 添加标题
            /// </summary>
            /// <param name="Title">标题</param>
            /// <param name="style">标题样式</param>
            /// <param name="rowHeight">行高</param>
            /// <param name="mergetAcross">跨行</param>
            /// <returns></returns>
            public Cell AppendTitle(string Title, Style style, double rowHeight = 24, int mergetAcross = 0)
            {
                if (mergetAcross == 0 && DataSource != null)
                {
                    mergetAcross = DataSource.Columns.Count - 1;
                }
                Row row = AppendRow();
                row.Height = rowHeight;
                return row.AddCell(Title, 1, style, mergetAcross);

            }
            ObservableCollection<DataValidation> dataValidations = new ObservableCollection<DataValidation>();
            [XmlElement(ElementName = "DataValidation")]
            public ObservableCollection<DataValidation> DataValidations
            {
                get { return dataValidations; }
                set
                {
                    if (value != dataValidations)
                    {
                        dataValidations = value;
                        NotifyPropertyChanged("DataValidation");
                    }
                }
            }

        }
        /// <summary>
        /// 预设区域
        /// </summary>
        public class NamedRange : INotifyPropertyChanged
        {
            public NamedRange()
            {

            }
            public NamedRange(string name)
            {
                Name = name;
            }
            string name;
            /// <summary>
            /// 区域名
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string Name
            {
                get { return name; }
                set
                {
                    if (value != name)
                    {
                        name = value;
                        NotifyPropertyChanged("Name");
                    }
                }
            }
            string refersTo;
            /// <summary>
            /// 预设区域的范围
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string RefersTo
            {
                get { return refersTo; }
                set
                {
                    if (value != refersTo)
                    {
                        refersTo = value;
                        NotifyPropertyChanged("RefersTo");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 工作表设置
        /// </summary>
        public class WorksheetOptions : INotifyPropertyChanged
        {
            PageSetup pageSetup = new PageSetup();
            /// <summary>
            /// 页面设置
            /// </summary>
            public PageSetup PageSetup
            {
                get { return pageSetup; }
                set
                {
                    if (value != pageSetup)
                    {
                        pageSetup = value;
                        NotifyPropertyChanged("PageSetup");
                    }
                }
            }
            bool fitToPage = false;
            [DefaultValue(false)]
            public bool FitToPage
            {
                get { return fitToPage; }
                set
                {
                    if (value != fitToPage)
                    {
                        fitToPage = value;
                        NotifyPropertyChanged("FitToPage");
                    }
                }
            }
            Print print = new Print();
            /// <summary>
            /// 打印设置
            /// </summary>
            public Print Print
            {
                get { return print; }
                set
                {
                    if (value != print)
                    {
                        print = value;
                        NotifyPropertyChanged("Print");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            /// <summary>
            /// 缩放打印宽度至一页宽
            /// </summary>
            public void SetWidthToOnPageWidth()
            {
                Print.FitHeight = Print.HorizontalResolution;
                FitToPage = true;
            }
            /// <summary>
            /// 缩放打印高度至一页高
            /// </summary>
            public void SetHeightToOnPageHeight()
            {
                Print.FitWidth = Print.VerticalResolution;
                FitToPage = true;
            }
            [DefaultValue(false)]
            /// <summary>
            /// 是否不显示网格线
            /// </summary>
            public bool DoNotDisplayGridlines = false;
            //public void SetToOnPage()
            //{
            //    Print.FitHeight = Print.HorizontalResolution;
            //    Print.FitWidth = Print.VerticalResolution;
            //    FitToPage = true;
            //}
        }
        /// <summary>
        /// 页面设置
        /// </summary>
        public class PageSetup : INotifyPropertyChanged
        {
            Layout layout = new Layout() { Orientation = Orientation.Portrait };
            /// <summary>
            /// 纸张布局
            /// </summary>
            public Layout Layout
            {
                get { return layout; }
                set
                {
                    if (value != layout)
                    {
                        layout = value;
                        NotifyPropertyChanged("Layout");
                    }
                }
            }
            Header header = new Header() { Margin = 0.3 };
            /// <summary>
            /// 页眉
            /// </summary>
            public Header Header
            {
                get { return header; }
                set
                {
                    if (value != header)
                    {
                        header = value;
                        NotifyPropertyChanged("Header");
                    }
                }
            }
            Header footer = new Header() { Margin = 0.3 };
            /// <summary>
            /// 页尾
            /// </summary>
            public Header Footer
            {
                get { return footer; }
                set
                {
                    if (value != footer)
                    {
                        footer = value;
                        NotifyPropertyChanged("Footer");
                    }
                }
            }
            PageMargins pageMargins = new PageMargins();
            /// <summary>
            /// 页面边距
            /// </summary>
            public PageMargins PageMargins
            {
                get { return pageMargins; }
                set
                {
                    if (value != pageMargins)
                    {
                        pageMargins = value;
                        NotifyPropertyChanged("PageMargins");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 纸张布局
        /// </summary>
        public class Layout : INotifyPropertyChanged
        {
            Orientation orientation = Orientation.Portrait;
            /// <summary>
            /// 纸张方向
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public Orientation Orientation
            {
                get { return orientation; }
                set
                {
                    if (value != orientation)
                    {
                        orientation = value;
                        NotifyPropertyChanged("Orientation");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 打印设置
        /// </summary>
        public class Print : INotifyPropertyChanged
        {

            int fitHeight = 0;
            /// <summary>
            /// 合适的高度（相对于打印解析度（默认600））
            /// </summary>
            [DefaultValue(0)]
            public int FitHeight
            {
                get { return fitHeight; }
                set
                {
                    if (value != fitHeight)
                    {
                        fitHeight = value;
                        NotifyPropertyChanged("FitHeight");
                    }
                }
            }
            int fitWidth = 0;
            /// <summary>
            /// 合适的宽度（相对于打印解析度（默认600））
            /// </summary>
            [DefaultValue(0)]
            public int FitWidth
            {
                get { return fitWidth; }
                set
                {
                    if (value != fitWidth)
                    {
                        fitWidth = value;
                        NotifyPropertyChanged("FitWidth");
                    }
                }
            }

            int paperSizeIndex = 9;
            /// <summary>
            /// 纸张尺寸类别（9为A4，8为A3，11为A5)
            /// </summary>
            [DefaultValue(0)]
            public int PaperSizeIndex
            {
                get { return paperSizeIndex; }
                set
                {
                    if (value != paperSizeIndex)
                    {
                        paperSizeIndex = value;
                        NotifyPropertyChanged("PaperSizeIndex");
                    }
                }
            }
            double scale = 100;
            /// <summary>
            /// 缩放尺寸（最大100）
            /// </summary>
            [DefaultValue(100)]
            public double Scale
            {
                get { return scale; }
                set
                {
                    if (value != scale)
                    {
                        scale = value;
                        NotifyPropertyChanged("Scale");
                    }
                }
            }
            public int HorizontalResolution = 600;
            public int VerticalResolution = 600;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        public class RowDataEventArgs : EventArgs
        {
            public DataRow Row { get; set; }
            public int RowIndex { get; set; }
            public Row CurrRow { get; set; }
            public Style CurrStyle { get; set; }
        }
        /// <summary>
        /// 自动过滤
        /// </summary>
        public class AutoFilter : INotifyPropertyChanged
        {
            string range;
            /// <summary>
            /// 自动过滤的范围
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public string Range
            {
                get { return range; }
                set
                {
                    if (value != range)
                    {
                        range = value;
                        NotifyPropertyChanged("Range");
                    }
                }
            }
            public AutoFilter()
            {

            }
            public AutoFilter(Range range)
            {
                Range = range.ToString();
            }
            ObservableCollection<AutoFilterColumn> autoFilterColumns;
            [XmlElement(ElementName = "AutoFilterColumn")]
            public ObservableCollection<AutoFilterColumn> AutoFilterColumns
            {
                get { return autoFilterColumns; }
                set
                {
                    if (autoFilterColumns != value)
                    {
                        autoFilterColumns = value;
                        NotifyPropertyChanged("AutoFilterColumns");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            /// <summary>
            /// 添加过滤条件
            /// </summary>
            /// <param name="columnIndex">指定列</param>
            /// <param name="value">过滤条件的值</param>
            /// <param name="filterOperator">过滤方式</param>
            public void AddFilter(int columnIndex, string value, FilterOperator filterOperator = FilterOperator.Equals)
            {
                if (AutoFilterColumns == null)
                {
                    AutoFilterColumns = new ObservableCollection<AutoFilterColumn>();
                }
                AutoFilterColumn autoFilterColumn = new AutoFilterColumn(columnIndex);
                autoFilterColumn.AutoFilterCondition = new AutoFilterCondition();
                autoFilterColumn.AutoFilterCondition.Operator = filterOperator;
                autoFilterColumn.AutoFilterCondition.Value = value;
                AutoFilterColumns.Add(autoFilterColumn);
            }
        }
        /// <summary>
        /// 过滤条件列
        /// </summary>
        public class AutoFilterColumn : INotifyPropertyChanged
        {
            int index = 1;
            /// <summary>
            /// 过滤条件所在的列
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public int Index
            {
                get { return index; }
                set
                {
                    if (value != index)
                    {
                        index = value;
                        NotifyPropertyChanged("Index");
                    }
                }
            }
            string type = "Custom";
            /// <summary>
            /// 过滤类型（自定义）
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public string Type
            {
                get { return type; }
                set
                {
                    if (value != type)
                    {
                        type = value;
                        NotifyPropertyChanged("Custom");
                    }
                }
            }
            AutoFilterCondition autoFilterCondition;
            /// <summary>
            /// 过滤条件
            /// </summary>
            public AutoFilterCondition AutoFilterCondition
            {
                get { return autoFilterCondition; }
                set
                {
                    if (value != autoFilterCondition)
                    {
                        autoFilterCondition = value;
                        NotifyPropertyChanged("AutoFilterCondition");
                    }
                }

            }

            [XmlArray]
            public List<AutoFilterCondition> AutoFilterAnd;

            public AutoFilterColumn()
            {

            }
            /// <summary>
            /// 初始化需要过滤条件的列
            /// </summary>
            /// <param name="columnIndex">列编号（从1开始）</param>
            public AutoFilterColumn(int columnIndex)
            {
                Index = columnIndex;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            /// <summary>
            /// 添加过滤条件
            /// </summary>
            /// <param name="value">过滤条件的值</param>
            /// <param name="filterOperator">过滤方法</param>
            public void AddFilter(object value, FilterOperator filterOperator = FilterOperator.Equals)
            {
                if (AutoFilterCondition == null && AutoFilterAnd == null)
                {
                    AutoFilterCondition = new AutoFilterCondition(value.ToString(), filterOperator);

                }
                else
                {

                    if (AutoFilterAnd == null)
                    {
                        AutoFilterAnd = new List<AutoFilterCondition>();
                        AutoFilterCondition cond = new AutoFilterCondition(AutoFilterCondition.Value, AutoFilterCondition.Operator);
                        AutoFilterAnd.Add(cond);
                        AutoFilterCondition = null;
                    }
                    AutoFilterCondition condition = new AutoFilterCondition(value.ToString(), filterOperator);
                    AutoFilterAnd.Add(condition);
                }
            }
        }
        /// <summary>
        /// 过滤条件
        /// </summary>
        public class AutoFilterCondition : INotifyPropertyChanged
        {
            FilterOperator opera = FilterOperator.Equals;
            /// <summary>
            /// 过滤方法
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public FilterOperator Operator
            {
                get { return opera; }
                set
                {
                    if (value != opera)
                    {
                        opera = value;
                        NotifyPropertyChanged("Operator");
                    }
                }
            }
            string value;
            /// <summary>
            /// 过滤条件的值
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public string Value
            {
                get { return value; }
                set
                {
                    if (this.value != value)
                    {
                        this.value = value;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
            public AutoFilterCondition()
            {

            }
            /// <summary>
            /// 自动过滤条件
            /// </summary>
            /// <param name="value">过滤值</param>
            /// <param name="filterOperator">过滤方法</param>
            public AutoFilterCondition(string value, FilterOperator filterOperator)
            {
                Value = value;
                Operator = filterOperator;
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 表
        /// </summary>
        public class Table : INotifyPropertyChanged
        {
            ObservableCollection<Column> columns;
            /// <summary>
            /// 列的集合（可添加也可不添加，添加时用于指定某一列的样式）
            /// </summary>
            [XmlElement(ElementName = "Column")]
            public ObservableCollection<Column> Columns
            {
                get { return columns; }
                set
                {
                    if (value != columns)
                    {
                        columns = value;
                        NotifyPropertyChanged("Columns");
                    }
                }
            }
            public Table()
            {

            }


            ObservableCollection<Row> rows = new ObservableCollection<Row>();
            /// <summary>
            /// 行的集合
            /// </summary>
            [XmlElement(ElementName = "Row")]
            public ObservableCollection<Row> Rows
            {
                get { return rows; }
                set
                {
                    if (value != rows)
                    {
                        rows = value;
                        NotifyPropertyChanged("Rows");
                    }
                }
            }
            double defaultColumnWidth = 54;
            /// <summary>
            /// 默认列宽
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public double DefaultColumnWidth
            {
                get { return defaultColumnWidth; }
                set
                {
                    if (value != defaultColumnWidth)
                    {
                        defaultColumnWidth = value;
                        NotifyPropertyChanged("DefaultColumnWidth");
                    }
                }
            }
            double defaultRowHeight = 18;
            /// <summary>
            /// 默认行高
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public double DefaultRowHeight
            {
                get { return defaultRowHeight; }
                set
                {
                    if (value != defaultRowHeight)
                    {
                        defaultRowHeight = value;
                        NotifyPropertyChanged("DefaultRowHeight");
                    }
                }
            }
            string styleID;
            /// <summary>
            /// 样式名
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string StyleID
            {
                get { return styleID; }
                set
                {
                    if (value != styleID)
                    {
                        styleID = value;
                        NotifyPropertyChanged("StyleID");
                    }
                }
            }

            Style style;
            /// <summary>
            /// 样式
            /// </summary>
            [XmlIgnore]
            public Style Style
            {
                get { return style; }
                set
                {
                    if (value != style)
                    {
                        style = value;
                        if (style != null)
                            StyleID = style.ID;
                        NotifyPropertyChanged("Style");
                    }
                }

            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 列
        /// </summary>
        public class Column : INotifyPropertyChanged
        {
            int index;
            /// <summary>
            /// 列序号（从1开始）
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int Index
            {
                get { return index; }
                set
                {
                    if (value != index)
                    {
                        index = value;
                        NotifyPropertyChanged("Index");
                    }
                }
            }
            int autoFitWidth = 1;
            /// <summary>
            /// 是否自动列宽（1为自动，0为不自动）
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int AutoFitWidth
            {
                get { return autoFitWidth; }
                set
                {
                    if (value != autoFitWidth)
                    {
                        autoFitWidth = value;
                        NotifyPropertyChanged("AutoFitWidth");
                    }
                }
            }
            double width = 54;
            /// <summary>
            /// 列宽
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public double Width
            {
                get { return width; }
                set
                {
                    if (value != width)
                    {
                        width = value;
                        NotifyPropertyChanged("Width");
                    }
                }
            }
            string styleID;
            /// <summary>
            /// 样式名
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string StyleID
            {
                get { return styleID; }
                set
                {
                    if (value != styleID)
                    {
                        styleID = value;
                        NotifyPropertyChanged("StyleID");
                    }
                }
            }
            Style style;
            /// <summary>
            /// 样式
            /// </summary>
            [XmlIgnore]
            public Style Style
            {
                get { return style; }
                set
                {
                    if (value != style)
                    {
                        style = value;
                        if (style != null)
                            StyleID = style.ID;
                        NotifyPropertyChanged("Style");
                    }
                }

            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

        }
        /// <summary>
        /// 行
        /// </summary>
        public class Row : INotifyPropertyChanged
        {
            int index;
            /// <summary>
            /// 行号（从1开始，添加行时必须按顺序添加）
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int Index
            {
                get { return index; }
                set
                {
                    if (value != index)
                    {
                        index = value;
                        NotifyPropertyChanged("Index");
                    }
                }
            }
            ObservableCollection<Cell> cells = new ObservableCollection<Cell>();
            /// <summary>
            /// 单元格集合
            /// </summary>
            [XmlElement(ElementName = "Cell")]
            public ObservableCollection<Cell> Cells
            {
                get { return cells; }
                set
                {
                    if (value != cells)
                    {
                        cells = value;
                        NotifyPropertyChanged("Cells");
                    }
                }
            }
            double height = 18;
            /// <summary>
            /// 行高
            /// </summary>
            [DefaultValue(18)]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public double Height
            {
                get { return height; }
                set
                {
                    if (value != height)
                    {
                        height = value;
                        NotifyPropertyChanged("Height");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            /// 添加单元格（必须根据序号按顺序添加）
            /// </summary>
            /// <param name="value">值</param>
            /// <param name="index">单元格所在列号（从1开始）</param>
            /// <param name="style">单元格样式</param>
            /// <param name="dataType">数据类型</param>
            /// <param name="mergeAcross">单元格跨列数</param>
            /// <param name="mergeDown">单元格跨行数</param>
            /// <returns></returns>
            public Cell AddCell(object value, int index = 0, Style style = null, int mergeAcross = 0, int mergeDown = 0, ExcelDataType dataType = ExcelDataType.Auto)
            {
                Cell cell = new Cell(value, index, style, dataType, mergeAcross, mergeDown);
                Cells.Add(cell);
                return cell;
            }

        }
        /// <summary>
        /// 单元格
        /// </summary>
        public class Cell : INotifyPropertyChanged
        {
            string formula;
            /// <summary>
            /// 单元格的公式
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string Formula
            {
                get { return formula; }
                set
                {
                    if (value != formula)
                    {
                        formula = value;
                        NotifyPropertyChanged("Formula");
                    }
                }
            }
            int index;
            /// <summary>
            /// 单元格序号（序号从1开始）
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int Index
            {
                get { return index; }
                set
                {
                    if (value != index)
                    {
                        index = value;
                        NotifyPropertyChanged("Index");
                    }
                }
            }
            Data data = new Data();
            /// <summary>
            /// 单元格数据
            /// </summary>
            public Data Data
            {
                get { return data; }
                set
                {
                    if (value != data)
                    {
                        data = value;
                        NotifyPropertyChanged("Data");
                    }
                }
            }
            int mergeAcross = 0;
            /// <summary>
            /// 单元格跨列数量（如数字为1则总共占用2个单元格的位置）
            /// </summary>
            [DefaultValue(0)]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int MergeAcross
            {
                get { return mergeAcross; }
                set
                {
                    if (value != mergeAcross)
                    {
                        mergeAcross = value;
                        NotifyPropertyChanged("MergeAcross");
                    }
                }
            }
            int mergeDown = 0;
            /// <summary>
            /// 单元格跨行数量（如数字为1则总共占用2个单元格的位置）
            /// </summary>
            [DefaultValue(0)]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int MergeDown
            {
                get { return mergeDown; }
                set
                {
                    if (value != mergeDown)
                    {
                        mergeDown = value;
                        NotifyPropertyChanged("MergeDown");
                    }
                }
            }

            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            string styleID;
            /// <summary>
            /// 样式编号
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string StyleID
            {
                get { return styleID; }
                set
                {
                    if (value != styleID)
                    {
                        styleID = value;
                        NotifyPropertyChanged("StyleID");
                    }
                }
            }
            Style style;
            /// <summary>
            /// 单元格样式
            /// </summary>
            [XmlIgnore]
            public Style Style
            {
                get { return style; }
                set
                {
                    if (value != style)
                    {
                        style = value;
                        if (style != null)
                            StyleID = style.ID;
                        NotifyPropertyChanged("Style");
                    }
                }

            }
            public Cell()
            {

            }
            /// <summary>
            /// 生成一个单元格
            /// </summary>
            /// <param name="value">单元格的内容</param>
            /// <param name="index">单元格序号（从1开始）</param>
            /// <param name="style">样式</param>
            /// <param name="dataType">类型</param>
            /// <param name="mergeAcross">单元格跨列数</param>
            /// <param name="mergeDown">单元格跨行数</param>
            public Cell(object value, int index = 0, Style style = null, ExcelDataType dataType = ExcelDataType.Auto, int mergeAcross = 0, int mergeDown = 0)
            {
                Style = style;
                Index = index;
                if (value == null)
                    value = "";
                Type type = value.GetType();
                if (dataType != ExcelDataType.Auto)
                {
                    Data.Type = dataType;
                }
                else if (type == typeof(uint) || type == typeof(UInt16) || type == typeof(UInt32)
                     || type == typeof(UInt64) || type == typeof(int) || type == typeof(Int16)
                     || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Single)
                     || type == typeof(Double) || type == typeof(Decimal))
                {
                    Data.Type = ExcelDataType.Number;
                }
                //else if (type == typeof(DateTime))
                //{
                //    Data.Type = ExcelDataType.DateTime;
                //}
                else
                {
                    Data.Type = ExcelDataType.String;
                }
                if (mergeAcross != 0)
                    MergeAcross = mergeAcross;
                if (mergeDown != 0)
                    MergeDown = mergeDown;
                Data.Value = value.ToString();
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        public class Data : INotifyPropertyChanged
        {
            ExcelDataType type = ExcelDataType.String;
            /// <summary>
            /// 单元格内容的类型（字符串、数字、日期）
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public ExcelDataType Type
            {
                get { return type; }
                set
                {
                    if (value != type)
                    {
                        type = value;
                        NotifyPropertyChanged("Type");
                    }
                }
            }
            string value;
            /// <summary>
            /// 单元格内容
            /// </summary>
            [XmlText]
            public string Value
            {
                get { return value; }
                set
                {
                    if (value != this.value)
                    {
                        this.value = value;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        public class Style : INotifyPropertyChanged
        {
            Interior background;
            /// <summary>
            /// 底纹样式
            /// </summary>
            [XmlElement(ElementName = "Interior")]
            public Interior Background
            {
                get { return background; }
                set
                {
                    if (value != background)
                    {
                        background = value;
                        NotifyPropertyChanged("Background");
                    }
                }
            }
            NumberFormat numberFormat = new NumberFormat();
            /// <summary>
            /// 数字格式
            /// </summary>
            public NumberFormat NumberFormat
            {
                get { return numberFormat; }
                set
                {
                    if (value != numberFormat)
                    {
                        numberFormat = value;
                        NotifyPropertyChanged("NumberFormat");
                    }
                }
            }
            Alignment alignment;
            /// <summary>
            /// 对齐方式
            /// </summary>
            public Alignment Alignment
            {
                get { return alignment; }
                set
                {
                    if (value != alignment)
                    {
                        alignment = value;
                        NotifyPropertyChanged("Alignment");
                    }
                }
            }
            /// <summary>
            /// 设置单元格对齐方式
            /// </summary>
            /// <param name="horizontal"></param>
            /// <param name="vertical"></param>
            public void SetAlignment(Horizontal horizontal, Vertical vertical = Vertical.Center)
            {
                Alignment = new Alignment(horizontal, vertical);
            }
            Font font;
            /// <summary>
            /// 字体
            /// </summary>
            public Font Font
            {
                get { return font; }
                set
                {
                    if (value != font)
                    {
                        font = value;
                        NotifyPropertyChanged("Font");
                    }
                }
            }
            string id = "";
            /// <summary>
            /// 样式名
            /// </summary>
            [XmlAttribute(AttributeName = "ID", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = Workbook.SPREADSHEETSTRING)]
            public string ID
            {
                get { return id; }
                set
                {
                    if (value != id)
                    {
                        id = value;
                        NotifyPropertyChanged("ID");
                    }
                }
            }
            ObservableCollection<Border> borders;
            /// <summary>
            /// 边框线条集合（最多左、上、右、下共4条）
            /// </summary>
            public ObservableCollection<Border> Borders
            {
                get { return borders; }
                set
                {
                    if (value != borders)
                    {
                        borders = value;
                        NotifyPropertyChanged("Borders");
                    }
                }
            }
            /// <summary>
            /// 初始化样式（样式名自动创建）
            /// </summary>
            public Style()
            {
                Thread.Sleep(20);
                ID = "S" + new Random().Next(99999999).ToString();
            }
            /// <summary>
            /// 初始化样式（指定样式名）
            /// </summary>
            /// <param name="id">样式名</param>
            public Style(string id)
            {
                if (id == "")
                {
                    Thread.Sleep(20);
                    ID = "S" + new Random().Next(99999999).ToString();
                }
                else
                    ID = id;
            }
            /// <summary>
            /// 设置边框及颜色
            /// </summary>
            /// <param name="lineWidth">线宽</param>
            /// <param name="color">颜色</param>
            public void SetBorders(int lineWidth, string color = "Black")
            {
                Borders = new ObservableCollection<Border>();
                Border bdrLeft = new Border() { Weight = lineWidth, Color = color, Position = Direct.Left };
                Border bdrTop = new Border() { Weight = lineWidth, Color = color, Position = Direct.Top };
                Border bdrRight = new Border() { Weight = lineWidth, Color = color, Position = Direct.Right };
                Border bdrBottom = new Border() { Weight = lineWidth, Color = color, Position = Direct.Bottom };

                Borders.Add(bdrLeft);
                Borders.Add(bdrTop);
                Borders.Add(bdrRight);
                Borders.Add(bdrBottom);
            }
            /// <summary>
            /// 设置边框及颜色
            /// </summary>
            /// <param name="lineWidth">线宽</param>
            /// <param name="color">颜色</param>
            public void SetBorders(int lineWidth, Colors colorBorder)
            {
                string color = colorBorder.ToString();
                Borders = new ObservableCollection<Border>();
                Border bdrLeft = new Border() { Weight = lineWidth, Color = color, Position = Direct.Left };
                Border bdrTop = new Border() { Weight = lineWidth, Color = color, Position = Direct.Top };
                Border bdrRight = new Border() { Weight = lineWidth, Color = color, Position = Direct.Right };
                Border bdrBottom = new Border() { Weight = lineWidth, Color = color, Position = Direct.Bottom };

                Borders.Add(bdrLeft);
                Borders.Add(bdrTop);
                Borders.Add(bdrRight);
                Borders.Add(bdrBottom);
            }
            /// <summary>
            /// 设置边框及颜色
            /// </summary>
            /// <param name="lineLeftWidth">左边框线宽</param>
            /// <param name="lineTopWidth">上边框线宽</param>
            /// <param name="lineRightWidth">右边框线宽</param>
            /// <param name="lineBottomWidth">下边框线宽</param>
            /// <param name="color">颜色</param>
            public void SetBorders(int lineLeftWidth, int lineTopWidth, int lineRightWidth, int lineBottomWidth, string color = "Black")
            {
                Borders = new ObservableCollection<Border>();
                Border bdrLeft = new Border() { Weight = lineLeftWidth, Color = color, Position = Direct.Left };
                Border bdrTop = new Border() { Weight = lineTopWidth, Color = color, Position = Direct.Top };
                Border bdrRight = new Border() { Weight = lineRightWidth, Color = color, Position = Direct.Right };
                Border bdrBottom = new Border() { Weight = lineBottomWidth, Color = color, Position = Direct.Bottom };

                Borders.Add(bdrLeft);
                Borders.Add(bdrTop);
                Borders.Add(bdrRight);
                Borders.Add(bdrBottom);
            }
            /// <summary>
            /// 复制样式
            /// </summary>
            /// <returns></returns>
            public Style Copy()
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Style));
                MemoryStream stream = new MemoryStream();
                xmlSerializer.Serialize(stream, this);
                stream.Position = 0;
                Style cellStyle = (Style)xmlSerializer.Deserialize(stream);
                Thread.Sleep(10);
                cellStyle.ID = "S" + new Random().Next(99999999).ToString();
                stream.Close();

                return cellStyle;
            }
            //public Style Copy()
            //{
            //    Style style = new Style();
            //    style.Font = Font;
            //    style.Borders = Borders;
            //    style.Background = Background;
            //    style.Alignment = Alignment;
            //    style.NumberFormat = NumberFormat;
            //    return style;
            //}
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 对齐方式
        /// </summary>
        public class Alignment : INotifyPropertyChanged
        {
            Horizontal horizontal = Horizontal.Left;
            /// <summary>
            /// 水平对齐方式
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public Horizontal Horizontal
            {
                get { return horizontal; }
                set
                {
                    if (value != horizontal)
                    {
                        horizontal = value;
                        NotifyPropertyChanged("Horizontal");
                    }
                }
            }
            Vertical vertical = Vertical.Center;
            /// <summary>
            /// 垂直对齐方式
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public Vertical Vertical
            {
                get { return vertical; }
                set
                {
                    if (value != vertical)
                    {
                        vertical = value;
                        NotifyPropertyChanged("Vertical");
                    }
                }
            }
            int wrapText = 0;
            /// <summary>
            /// 是否自动换行（0为不自动换行，1为自动换行）
            /// </summary>
            [DefaultValue(0)]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int WrapText
            {
                get { return wrapText; }
                set
                {
                    if (value != wrapText)
                    {
                        wrapText = value;
                        NotifyPropertyChanged("WrapText");
                    }
                }
            }
            public Alignment()
            {

            }
            public Alignment(Horizontal horizontal)
            {
                Horizontal = horizontal;
            }
            public Alignment(Horizontal horizontal, Vertical vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        public class Font : INotifyPropertyChanged
        {

            string fontName = "宋体";
            /// <summary>
            /// 字体名
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string FontName
            {
                get { return fontName; }
                set
                {
                    if (value != fontName)
                    {
                        fontName = value;
                        NotifyPropertyChanged("FontName");
                    }
                }
            }
            double size = 11;
            /// <summary>
            /// 字体尺寸
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public double Size
            {
                get { return size; }
                set
                {
                    if (value != size)
                    {
                        size = value;
                        NotifyPropertyChanged("Size");
                    }
                }
            }
            string color = "Black";
            /// <summary>
            /// 字体颜色
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string Color
            {
                get { return color; }
                set
                {
                    if (value != color)
                    {
                        color = value;
                        NotifyPropertyChanged("Color");
                    }
                }
            }
            int bold = 0;
            /// <summary>
            /// 加粗时填1，不加粗时填0
            /// </summary>
            [DefaultValue(0)]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int Bold
            {
                get { return bold; }
                set
                {
                    if (value != bold)
                    {
                        bold = value;
                        NotifyPropertyChanged("Bold");
                    }
                }
            }
            public Font()
            {

            }
            public Font(string fontName, double size = 11, string color = "Black", bool IsBold = false)
            {

                this.FontName = fontName;
                this.Size = size;
                Color = color;
                Bold = IsBold == true ? 1 : 0;
            }
            public Font(string fontName, double size = 11, Colors color = Colors.Black, bool IsBold = false)
            {

                this.FontName = fontName;
                this.Size = size;
                Color = color.ToString();
                Bold = IsBold == true ? 1 : 0;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 单元格底纹
        /// </summary>
        public class Interior : INotifyPropertyChanged
        {
            string color;
            /// <summary>
            /// 底纹背景颜色
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string Color
            {
                get { return color; }
                set
                {
                    if (value != color)
                    {
                        color = value;
                        NotifyPropertyChanged("Color");
                    }
                }
            }
            PatternStyle pattern = PatternStyle.None;
            /// <summary>
            /// 底纹样式
            /// </summary>
            [DefaultValue(PatternStyle.None)]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public PatternStyle Pattern
            {
                get { return pattern; }
                set
                {
                    if (value != pattern)
                    {
                        pattern = value;
                        NotifyPropertyChanged("Pattern");
                    }
                }
            }
            string patternColor = "Black";
            [DefaultValue("Black")]
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string PatternColor
            {
                get { return patternColor; }
                set
                {
                    if (value != patternColor)
                    {
                        patternColor = value;
                        NotifyPropertyChanged("PatternColor");
                    }
                }
            }
            public Interior()
            {

            }
            /// <summary>
            /// 以指定的颜色初始化底纹
            /// </summary>
            /// <param name="color"></param>
            public Interior(string color)
            {
                Color = color;
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 边框样式
        /// </summary>
        public class Border : INotifyPropertyChanged
        {
            string color = "Black";
            /// <summary>
            /// 线条颜色
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string Color
            {
                get { return color; }
                set
                {
                    if (value != color)
                    {
                        color = value;
                        NotifyPropertyChanged("Color");
                    }
                }
            }
            Direct position;
            /// <summary>
            /// 线条方向
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public Direct Position
            {
                get { return position; }
                set
                {
                    if (value != position)
                    {
                        position = value;
                        NotifyPropertyChanged("Position");
                    }
                }
            }
            BorderLineStyle lineStyle = BorderLineStyle.Continuous;
            /// <summary>
            /// 线条样式
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public BorderLineStyle LineStyle
            {
                get { return lineStyle; }
                set
                {
                    if (lineStyle != value)
                    {
                        lineStyle = value;
                        NotifyPropertyChanged("LineStyle");
                    }
                }
            }
            int weight = 0;
            /// <summary>
            /// 线宽
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public int Weight
            {
                get { return weight; }
                set
                {
                    if (value != weight)
                    {
                        weight = value;
                        NotifyPropertyChanged("Weight");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

        }
        /// <summary>
        /// 数字格式
        /// </summary>
        public class NumberFormat : INotifyPropertyChanged
        {

            /// <summary>
            /// 会计专用数字格式（带千位分隔符）
            /// </summary>
            public const string NumberFormatKJ = "#,##0.00";
            /// <summary>
            /// 普通数字格式（保留2位小数）
            /// </summary>
            public const string NumberFormatNormal = "0.00";
            /// <summary>
            /// 文本格式
            /// </summary>
            public const string StringFormat = "@";
            /// <summary>
            /// 日期格式
            /// </summary>
            public const string DateFormat = "yyyy-MM-dd";

            string format;
            /// <summary>
            /// 格式
            /// </summary>
            [XmlAttribute(Namespace = Workbook.SPREADSHEETSTRING)]
            public string Format
            {
                get { return format; }
                set
                {
                    if (value != format)
                    {
                        format = value;
                        NotifyPropertyChanged("Format");
                    }
                }
            }
            public NumberFormat()
            {

            }
            public NumberFormat(string format)
            {
                Format = format;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        /// <summary>
        /// 页眉（页脚）
        /// </summary>
        public class Header : INotifyPropertyChanged
        {
            /// <summary>
            /// 当前页码
            /// </summary>
            public const string CurrPage = "&P";
            /// <summary>
            /// 总页码
            /// </summary>
            public const string TotalPage = "&N";
            /// <summary>
            /// 当前日期
            /// </summary>
            public const string CurrDate = "&D";
            /// <summary>
            /// 当前时间
            /// </summary>
            public const string CurrTime = "&T";
            double margin;
            /// <summary>
            /// 边距
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public double Margin
            {
                get { return margin; }
                set
                {
                    if (value != margin)
                    {
                        margin = value;
                        NotifyPropertyChanged("Margin");
                    }
                }
            }
            string data;
            /// <summary>
            /// 页眉（页脚）的内容以字符串形式表示
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public string Data
            {
                get { return data; }
                set
                {
                    if (value != data)
                    {
                        data = value;
                        NotifyPropertyChanged("Data");
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
            /// <summary>
            /// 添加内容
            /// </summary>
            /// <param name="text">页眉（页脚）的内容</param>
            /// <param name="horizontal">水平位置（左、中、右）</param>
            /// <param name="textFont">文字字体</param>
            public void AddText(string text, Horizontal horizontal, Font textFont = null)
            {
                if (Data == null)
                    Data = "";
                Data += "&" + horizontal.ToString()[0];
                if (textFont != null)
                {
                    string str = "&\"" + textFont.FontName;
                    if (textFont.Bold == 1)
                        str += ",加粗\"";
                    else
                        str += "\"";
                    str += ("&" + textFont.Size);
                    if (textFont.Color != "Black")
                    {
                        string col = textFont.Color;
                        if (textFont.Color[0] == '#')
                        {
                            col = "K" + textFont.Color.Substring(1);
                        }
                        else
                        {
                            Colors color = Colors.Black;
                            Enum.TryParse(textFont.Color, out color);
                            col = Convert.ToString((int)color, 16);
                            col = "K" + col.Substring(2);
                        }
                        str += ("&" + col);
                    }
                    Data += str;
                }
                Data += text;

            }
        }
        /// <summary>
        /// 页面边距
        /// </summary>
        public class PageMargins : INotifyPropertyChanged
        {
            /// <summary>
            /// 初始化页边距
            /// </summary>
            public PageMargins()
            {

            }
            /// <summary>
            /// 初始化页边距为相同的值
            /// </summary>
            /// <param name="value"></param>
            public PageMargins(double value)
            {
                Left = value;
                Top = value;
                Right = value;
                Bottom = value;
            }
            public PageMargins(double left, double top, double right, double bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
            double left = 0.7;
            /// <summary>
            /// 左边距
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public double Left
            {
                get { return left; }
                set
                {
                    if (value != left)
                    {
                        left = value;
                        NotifyPropertyChanged("Left");
                    }
                }
            }

            double top = 0.75;
            /// <summary>
            /// 上边距
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public double Top
            {
                get { return top; }
                set
                {
                    if (value != top)
                    {
                        top = value;
                        NotifyPropertyChanged("Top");
                    }
                }
            }
            double right = 0.7;
            /// <summary>
            /// 右边距
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public double Right
            {
                get { return right; }
                set
                {
                    if (value != right)
                    {
                        right = value;
                        NotifyPropertyChanged("Right");
                    }
                }
            }
            double bottom = 0.75;
            /// <summary>
            /// 下边距
            /// </summary>
            [XmlAttribute(Namespace = Workbook.EXCELSTRING)]
            public double Bottom
            {
                get { return bottom; }
                set
                {
                    if (value != bottom)
                    {
                        bottom = value;
                        NotifyPropertyChanged("Bottom");
                    }
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }

        /// <summary>
        /// Excel单元格范围
        /// </summary>
        public class Range : IXmlSerializable
        {
            public int? RowStart;
            public int? ColumnStart;
            public int? RowEnd;
            public int? ColumnEnd;
            [XmlIgnore]
            public Worksheet Worksheet;
            [XmlIgnore]
            public string WorkSheetName;
            public Range()
            {

            }
            /// <summary>
            /// 初始化范围
            /// </summary>
            /// <param name="start">起始</param>
            /// <param name="end">终止</param>
            /// <param name="isRow">是否为行，false时为列</param>
            public Range(int start, int end, bool isRow = true)
            {
                if (isRow == true)
                {
                    RowStart = start;
                    RowEnd = end;
                }
                else
                {
                    ColumnStart = start;
                    ColumnEnd = end;
                }
            }
            /// <summary>
            /// 初始化单元格范围
            /// </summary>
            /// <param name="rowStart">行起始序号（从1开始）</param>
            /// <param name="columnStart">列起始序号（从1开始）</param>
            /// <param name="rowEnd">行终止序号（从1开始）</param>
            /// <param name="columnEnd">列终止序号（从1开始）</param>
            /// <param name="worksheet">所在表</param>
            public Range(int? rowStart, int? columnStart, int? rowEnd, int? columnEnd, Worksheet worksheet)
            {
                RowStart = rowStart;
                ColumnStart = columnStart;
                RowEnd = rowEnd;
                ColumnEnd = columnEnd;
                this.Worksheet = worksheet;
            }
            /// <summary>
            /// 初始化单元格范围
            /// </summary>
            /// <param name="rowStart">行起始序号（从1开始）</param>
            /// <param name="columnStart">列起始序号（从1开始）</param>
            /// <param name="rowEnd">行终止序号（从1开始）</param>
            /// <param name="columnEnd">列终止序号（从1开始）</param>
            /// <param name="worksheet">所在表</param>
            public Range(int? rowStart, int? columnStart, int? rowEnd, int? columnEnd, string worksheetName = null)
            {
                RowStart = rowStart;
                ColumnStart = columnStart;
                RowEnd = rowEnd;
                ColumnEnd = columnEnd;
                WorkSheetName = worksheetName;
            }
            public override string ToString()
            {
                string str = "";
                if (Worksheet != null)
                {
                    str = Worksheet.Name + "!";
                }
                else if (!string.IsNullOrWhiteSpace(WorkSheetName))
                    str += WorkSheetName + "!";
                str += ((RowStart.HasValue == false ? "" : ("R" + RowStart.ToString())) + (ColumnStart.HasValue == false ? "" : ("C" + ColumnStart.ToString()))
                    + ":" + (RowEnd.HasValue == false ? "" : ("R" + RowEnd.ToString())) + (ColumnEnd.HasValue == false ? "" : ("C" + ColumnEnd.ToString())));
                return str;
            }
            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                try
                {
                    string str = reader.ReadString();
                    if (str.Contains("!"))
                    {
                        string s = str.Split('!')[0];
                        WorkSheetName = s;
                        str = s.Split('!')[1];
                    }
                    string[] ss = str.Split(':');
                    Regex rex = new Regex(@"(?<=R)\d+");
                    Match m = rex.Match(ss[0]);
                    if (m.Success == true)
                    {
                        RowStart = int.Parse(m.Value);
                    }
                    rex = new Regex(@"(?<=C)\d+");
                    m = rex.Match(ss[0]);
                    if (m.Success == true)
                    {
                        ColumnStart = int.Parse(m.Value);
                    }
                    if (ss.Length > 1)
                    {
                        rex = new Regex(@"(?<=R)\d+");
                        m = rex.Match(ss[1]);
                        if (m.Success == true)
                        {
                            RowEnd = int.Parse(m.Value);
                        }
                        rex = new Regex(@"(?<=C)\d+");
                        m = rex.Match(ss[1]);
                        if (m.Success == true)
                        {
                            ColumnEnd = int.Parse(m.Value);
                        }
                    }
                    else
                    {
                        RowEnd = RowStart;
                        ColumnEnd = ColumnStart;
                    }
                }
                catch (Exception err)
                {

                }
            }
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteString(this.ToString());
            }

        }

        /// <summary>
        /// 数据验证
        /// </summary>
        public class DataValidation : INotifyPropertyChanged
        {
            public DataValidation()
            {

            }
            public DataValidation(string[] list, Range range)
            {
                Value = "\"" + string.Join(",", list) + "\"";
                Range = range;
            }
            public void SetTip(string title, string message)
            {
                InputTitle = title;
                InputMessage = message;
            }
            public void SetErrorMessage(string title, string message)
            {
                errorTitle = title;
                errorMessage = message;
            }

            Range range;
            /// <summary>
            /// 数据区
            /// </summary>
            public Range Range
            {
                get { return range; }
                set
                {
                    if (value != range)
                    {
                        range = value;
                        NotifyPropertyChanged("Range");
                    }
                }
            }
            DataValidDataType type = DataValidDataType.List;
            /// <summary>
            /// 数据有效性类型
            /// </summary>
            public DataValidDataType Type
            {
                get { return type; }
                set
                {
                    if (value != type)
                    {
                        type = value;
                        NotifyPropertyChanged("Type");
                    }
                }
            }
            string val;
            /// <summary>
            /// 值
            /// </summary>
            public string Value
            {
                get { return val; }
                set
                {
                    if (value != val)
                    {
                        val = value;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
            string min;
            /// <summary>
            /// 最小值
            /// </summary>
            public string Min
            {
                get { return min; }
                set
                {
                    if (value != min)
                    {
                        min = value;
                        NotifyPropertyChanged("Min");
                    }
                }
            }
            string max;
            /// <summary>
            /// 最大值
            /// </summary>
            public string Max
            {
                get { return max; }
                set
                {
                    if (value != max)
                    {
                        max = value;
                        NotifyPropertyChanged("Max");
                    }
                }
            }
            string inputTitle;
            /// <summary>
            /// 提示标题
            /// </summary>
            public string InputTitle
            {
                get { return inputTitle; }
                set
                {
                    if (value != inputTitle)
                    {
                        inputTitle = value;
                        NotifyPropertyChanged("InputTitle");
                    }
                }
            }
            string inputMessage;
            /// <summary>
            /// 提示信息
            /// </summary>
            public string InputMessage
            {
                get { return inputMessage; }
                set
                {
                    if (value != inputMessage)
                    {
                        inputMessage = value;
                        NotifyPropertyChanged("InputMessage");
                    }

                }
            }
            ErrorStyle errorStyle = ErrorStyle.Stop;
            /// <summary>
            /// 错误图标样式
            /// </summary>
            public ErrorStyle ErrorStyle
            {
                get { return errorStyle; }
                set
                {
                    if (value != errorStyle)
                    {
                        errorStyle = value;
                        NotifyPropertyChanged("ErrorStyle");
                    }
                }
            }
            string errorMessage;
            /// <summary>
            /// 错误信息
            /// </summary>
            public string ErrorMessage
            {
                get { return errorMessage; }
                set
                {
                    if (value != errorMessage)
                    {
                        errorMessage = value;
                        NotifyPropertyChanged("ErrorMessage");
                    }
                }
            }
            string errorTitle;
            /// <summary>
            /// 错误标题
            /// </summary>
            public string ErrorTitle
            {
                get { return errorTitle; }
                set
                {
                    if (value != errorTitle)
                    {
                        errorTitle = value;
                        NotifyPropertyChanged("ErrorTitle");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }
        }
        #region 枚举类型定义
        /// <summary>
        /// 边框线样式
        /// </summary>
        public enum BorderLineStyle
        {
            /// <summary>
            /// 单实线
            /// </summary>
            Continuous,
            /// <summary>
            /// 点
            /// </summary>
            Dot,
            Dash,
            /// <summary>
            /// 双实线
            /// </summary>
            Double,
            DashDot,
            DashDotDot,
            SlantDashDot
        }
        /// <summary>
        /// 数据有效性验证类型
        /// </summary>
        public enum DataValidDataType
        {
            List,
            Whole,
            Decimal,
            Date,
            Time,
            TextLength
        }
        /// <summary>
        /// 数据验证错误类型
        /// </summary>
        public enum ErrorStyle
        {
            Stop,
            Warn,
            Info
        }
        /// <summary>
        /// 底纹类型
        /// </summary>
        public enum PatternStyle
        {
            /// <summary>
            /// 无底纹
            /// </summary>
            None,
            /// <summary>
            /// 纯色
            /// </summary>
            Solid,
            ThinHorzCross,
            ThinDiagCross,
            ThinHorzStripe,
            ThinVertStripe,
            Gray0625

        }
        public enum Direct
        {
            Left,
            Top,
            Right,
            Bottom
        }
        /// <summary>
        /// 水平对齐
        /// </summary>
        public enum Horizontal
        {
            /// <summary>
            /// 左
            /// </summary>
            Left,
            /// <summary>
            /// 中
            /// </summary>
            Center,
            /// <summary>
            /// 右
            /// </summary>
            Right
        }
        public enum Vertical
        {
            /// <summary>
            /// 上
            /// </summary>
            Top,
            /// <summary>
            /// 中
            /// </summary>
            Center,
            /// <summary>
            /// 下
            /// </summary>
            Bottom
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public enum ExcelDataType
        {
            /// <summary>
            /// 自动
            /// </summary>
            Auto,
            /// <summary>
            /// 字符串
            /// </summary>
            String,
            /// <summary>
            /// 数值
            /// </summary>
            Number,
            /// <summary>
            /// 时间
            /// </summary>
            DateTime
        }
        /// <summary>
        /// 纸张方向
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            /// 横向
            /// </summary>
            Portrait,
            /// <summary>
            /// 纵向
            /// </summary>
            Landscape
        }
        /// <summary>
        /// 过滤操作
        /// </summary>
        public enum FilterOperator
        {
            /// <summary>
            /// 等于（如取包含值则用*和?表示，*为多个字符，?为单个字符)
            /// </summary>
            Equals,
            /// <summary>
            /// 不等于（如取包含值则用*和?表示，*为多个字符，?为单个字符)
            /// </summary>
            DoesNotEqual,
            /// <summary>
            /// 大于
            /// </summary>
            GreaterThan,
            /// <summary>
            /// 小于
            /// </summary>
            LessThan,
        }
        public enum Colors : uint
        {
            // We've reserved the value "1" as unknown.  If for some odd reason "1" is added to the
            // list, redefined UnknownColor

            AliceBlue = 0xFFF0F8FF,
            AntiqueWhite = 0xFFFAEBD7,
            Aqua = 0xFF00FFFF,
            Aquamarine = 0xFF7FFFD4,
            Azure = 0xFFF0FFFF,
            Beige = 0xFFF5F5DC,
            Bisque = 0xFFFFE4C4,
            Black = 0xFF000000,
            BlanchedAlmond = 0xFFFFEBCD,
            Blue = 0xFF0000FF,
            BlueViolet = 0xFF8A2BE2,
            Brown = 0xFFA52A2A,
            BurlyWood = 0xFFDEB887,
            CadetBlue = 0xFF5F9EA0,
            Chartreuse = 0xFF7FFF00,
            Chocolate = 0xFFD2691E,
            Coral = 0xFFFF7F50,
            CornflowerBlue = 0xFF6495ED,
            Cornsilk = 0xFFFFF8DC,
            Crimson = 0xFFDC143C,
            Cyan = 0xFF00FFFF,
            DarkBlue = 0xFF00008B,
            DarkCyan = 0xFF008B8B,
            DarkGoldenrod = 0xFFB8860B,
            DarkGray = 0xFFA9A9A9,
            DarkGreen = 0xFF006400,
            DarkKhaki = 0xFFBDB76B,
            DarkMagenta = 0xFF8B008B,
            DarkOliveGreen = 0xFF556B2F,
            DarkOrange = 0xFFFF8C00,
            DarkOrchid = 0xFF9932CC,
            DarkRed = 0xFF8B0000,
            DarkSalmon = 0xFFE9967A,
            DarkSeaGreen = 0xFF8FBC8F,
            DarkSlateBlue = 0xFF483D8B,
            DarkSlateGray = 0xFF2F4F4F,
            DarkTurquoise = 0xFF00CED1,
            DarkViolet = 0xFF9400D3,
            DeepPink = 0xFFFF1493,
            DeepSkyBlue = 0xFF00BFFF,
            DimGray = 0xFF696969,
            DodgerBlue = 0xFF1E90FF,
            Firebrick = 0xFFB22222,
            FloralWhite = 0xFFFFFAF0,
            ForestGreen = 0xFF228B22,
            Fuchsia = 0xFFFF00FF,
            Gainsboro = 0xFFDCDCDC,
            GhostWhite = 0xFFF8F8FF,
            Gold = 0xFFFFD700,
            Goldenrod = 0xFFDAA520,
            Gray = 0xFF808080,
            Green = 0xFF008000,
            GreenYellow = 0xFFADFF2F,
            Honeydew = 0xFFF0FFF0,
            HotPink = 0xFFFF69B4,
            IndianRed = 0xFFCD5C5C,
            Indigo = 0xFF4B0082,
            Ivory = 0xFFFFFFF0,
            Khaki = 0xFFF0E68C,
            Lavender = 0xFFE6E6FA,
            LavenderBlush = 0xFFFFF0F5,
            LawnGreen = 0xFF7CFC00,
            LemonChiffon = 0xFFFFFACD,
            LightBlue = 0xFFADD8E6,
            LightCoral = 0xFFF08080,
            LightCyan = 0xFFE0FFFF,
            LightGoldenrodYellow = 0xFFFAFAD2,
            LightGreen = 0xFF90EE90,
            LightGray = 0xFFD3D3D3,
            LightPink = 0xFFFFB6C1,
            LightSalmon = 0xFFFFA07A,
            LightSeaGreen = 0xFF20B2AA,
            LightSkyBlue = 0xFF87CEFA,
            LightSlateGray = 0xFF778899,
            LightSteelBlue = 0xFFB0C4DE,
            LightYellow = 0xFFFFFFE0,
            Lime = 0xFF00FF00,
            LimeGreen = 0xFF32CD32,
            Linen = 0xFFFAF0E6,
            Magenta = 0xFFFF00FF,
            Maroon = 0xFF800000,
            MediumAquamarine = 0xFF66CDAA,
            MediumBlue = 0xFF0000CD,
            MediumOrchid = 0xFFBA55D3,
            MediumPurple = 0xFF9370DB,
            MediumSeaGreen = 0xFF3CB371,
            MediumSlateBlue = 0xFF7B68EE,
            MediumSpringGreen = 0xFF00FA9A,
            MediumTurquoise = 0xFF48D1CC,
            MediumVioletRed = 0xFFC71585,
            MidnightBlue = 0xFF191970,
            MintCream = 0xFFF5FFFA,
            MistyRose = 0xFFFFE4E1,
            Moccasin = 0xFFFFE4B5,
            NavajoWhite = 0xFFFFDEAD,
            Navy = 0xFF000080,
            OldLace = 0xFFFDF5E6,
            Olive = 0xFF808000,
            OliveDrab = 0xFF6B8E23,
            Orange = 0xFFFFA500,
            OrangeRed = 0xFFFF4500,
            Orchid = 0xFFDA70D6,
            PaleGoldenrod = 0xFFEEE8AA,
            PaleGreen = 0xFF98FB98,
            PaleTurquoise = 0xFFAFEEEE,
            PaleVioletRed = 0xFFDB7093,
            PapayaWhip = 0xFFFFEFD5,
            PeachPuff = 0xFFFFDAB9,
            Peru = 0xFFCD853F,
            Pink = 0xFFFFC0CB,
            Plum = 0xFFDDA0DD,
            PowderBlue = 0xFFB0E0E6,
            Purple = 0xFF800080,
            Red = 0xFFFF0000,
            RosyBrown = 0xFFBC8F8F,
            RoyalBlue = 0xFF4169E1,
            SaddleBrown = 0xFF8B4513,
            Salmon = 0xFFFA8072,
            SandyBrown = 0xFFF4A460,
            SeaGreen = 0xFF2E8B57,
            SeaShell = 0xFFFFF5EE,
            Sienna = 0xFFA0522D,
            Silver = 0xFFC0C0C0,
            SkyBlue = 0xFF87CEEB,
            SlateBlue = 0xFF6A5ACD,
            SlateGray = 0xFF708090,
            Snow = 0xFFFFFAFA,
            SpringGreen = 0xFF00FF7F,
            SteelBlue = 0xFF4682B4,
            Tan = 0xFFD2B48C,
            Teal = 0xFF008080,
            Thistle = 0xFFD8BFD8,
            Tomato = 0xFFFF6347,
            Transparent = 0x00FFFFFF,
            Turquoise = 0xFF40E0D0,
            Violet = 0xFFEE82EE,
            Wheat = 0xFFF5DEB3,
            White = 0xFFFFFFFF,
            WhiteSmoke = 0xFFF5F5F5,
            Yellow = 0xFFFFFF00,
            YellowGreen = 0xFF9ACD32,
            UnknownColor = 0x00000001
        }
        #endregion
        public static class ExcelFunc
        {
            public static DataTable GetDataTableFromFile(string file, string[] columns = null, string spliteText = "制表符", string encodingName = null)
            {
                try
                {
                    var encoding = Encoding.GetEncoding("GB2312");
                    if (!string.IsNullOrWhiteSpace(encodingName))
                    {
                        encoding = Encoding.GetEncoding(encodingName);
                    }

                    DataTable dt = new DataTable("Table");
                    //char c = ' ';
                    //switch (spliteText)
                    //{
                    //    case "制表符":
                    //        c = '\t';
                    //        break;
                    //    default:

                    //        break;
                    //}
                    //if (c == ' ')
                    //{
                    //    if (spliteText.Length == 1)
                    //    {
                    //        c = spliteText[0];
                    //    }
                    //    else
                    //    {
                    //        return null;
                    //    }
                    //}
                    string Err = string.Empty;

                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            while (!reader.EndOfStream)
                            {
                                string str = reader.ReadLine();
                                str = str.Trim();
                                string[] strs = str.Split('\t');
                                if (dt.Columns.Count == 0)
                                {
                                    for (int i = 0; i < strs.Length; i++)
                                    {
                                        if (columns != null)
                                        {
                                            if (strs.Length != columns.Length)
                                            {
                                                Err = "下载本地成功！导入数据库失败！列标题:" + columns.Length.ToString() + "和内容:" + strs.Length.ToString() + "不一致! ,将用空字符串代替";
                                                List<string> newlist = strs.ToList();
                                                int cz = Math.Abs(columns.Length - strs.Length);
                                                for (int k = 0; k < cz; k++)
                                                {
                                                    newlist.Add("");
                                                }
                                                strs = newlist.ToArray();
                                                //strs[strs.Length] = "";
                                                //return null;
                                            }
                                            //else
                                            //{
                                            dt.Columns.Add(columns[i]);
                                            //}
                                        }
                                        else
                                            dt.Columns.Add("C" + (i + 1).ToString());
                                    }
                                }
                                dt.Rows.Add(strs);
                            }
                            if (!string.IsNullOrEmpty(Err))
                            {
                                MessageBox.Show(Err, "提示");
                            }
                        }
                    }
                    return dt;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "转换失败");
                    return null;
                }
            }

            public static DataTable GetDataTableFromFile2(string file, string isyp, string[] columns = null, string spliteText = "制表符", string encodingName = null)
            {
                try
                {
                    var encoding = Encoding.GetEncoding("gb2312");
                    if (!string.IsNullOrWhiteSpace(encodingName))
                    {
                        encoding = Encoding.GetEncoding(encodingName);
                    }

                    DataTable dt = new DataTable("Table");
                    char c = ' ';
                    switch (spliteText)
                    {
                        case "制表符":
                            c = '\t';
                            break;
                        default:

                            break;
                    }
                    if (c == ' ')
                    {
                        if (spliteText.Length == 1)
                        {
                            c = spliteText[0];
                        }
                        else
                        {
                            return null;
                        }
                    }


                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream, encoding))
                        {
                            while (!reader.EndOfStream)
                            {
                                string str = reader.ReadLine();
                                string[] strs = str.Split(c);
                                if (dt.Columns.Count == 0)
                                {
                                    int cd = 0;
                                    if (isyp != "2")
                                        cd = strs.Length;
                                    else
                                        cd = columns.Length;
                                    for (int i = 0; i < cd; i++)
                                    {
                                        if (columns != null)
                                        {
                                            if (isyp != "2")
                                            {
                                                if (strs.Length != columns.Length)
                                                {
                                                    MessageBox.Show("列标题:" + columns.Length.ToString() + "和内容:" + strs.Length.ToString() + "不一致!", "转换失败");
                                                    return null;
                                                }
                                                else
                                                {
                                                    dt.Columns.Add(columns[i]);
                                                }
                                            }
                                            else
                                            {
                                                dt.Columns.Add(columns[i]);
                                            }
                                        }
                                        else
                                            dt.Columns.Add("C" + (i + 1).ToString());
                                    }
                                }
                                string ypno = "";
                                string ypnm = "";
                                string gg = "";
                                string strSql = "";

                                if (isyp == "1")
                                {
                                    ypno = strs[7];
                                    ypnm = strs[8];
                                    gg = strs[11];
                                    strSql = string.Format(@"select 1 from ybmrdr where dm='{0}' and dmmc='{1}' and gg='{2}'", ypno, ypnm, gg);

                                }
                                else if (isyp == "2")
                                {
                                    ypno = strs[0];
                                    strSql = string.Format(@"select 1 from ybbzmrdr where dm='{0}'", ypno);
                                    List<string> list = strs.ToList();
                                    //2 4 7 8 9 10
                                    list.RemoveAt(2);
                                    list.RemoveAt(3);
                                    list.RemoveAt(5);
                                    list.RemoveAt(5);
                                    list.RemoveAt(5);
                                    list.RemoveAt(5);
                                    strs = list.ToArray();
                                }
                                else
                                {
                                    ypno = strs[0];
                                    ypnm = strs[1];
                                    strSql = string.Format(@"select 1 from ybmrdr where dm='{0}' and dmmc='{1}' ", ypno, ypnm);
                                }
                                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                                if (ds.Tables[0].Rows.Count <= 0)
                                    dt.Rows.Add(strs);
                                ds.Dispose();
                            }
                        }
                    }
                    return dt;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "转换失败");
                    return null;
                }
            }

            public static void Export(DataTable dt, string filePath, string Title = "", string LeftHeader1 = "", string RightHeader1 = "", string LeftHeader2 = "", string RightHeader2 = "", string LeftFooter = "", string RightFooter = "", Action<Workbook, Worksheet> actionAfter = null)
            {
                Workbook workbook = new Workbook();
                Worksheet sheet = workbook.AppendSheet("工作表");
                sheet.Table = new Table();

                sheet.DataSource = dt;

                Style styleTitle = workbook.CreateStyle();
                styleTitle.Alignment = new Alignment(Horizontal.Center);
                styleTitle.Font = new Font() { Size = 16, FontName = "宋体", Bold = 1, Color = "Black" };
                Row rowTitle = sheet.AppendRow();
                rowTitle.Height = 26;
                Cell cellTitle = new Cell(Title, 1, styleTitle);
                cellTitle.MergeAcross = dt.Columns.Count - 1;
                rowTitle.Cells.Add(cellTitle);

                if (LeftHeader1 != "" || RightHeader1 != "")
                {
                    Row rowDesc = sheet.AppendRow();
                    Cell cellH1 = new Cell(LeftHeader1, 1);
                    rowDesc.Cells.Add(cellH1);

                    Style styleRight = workbook.CreateStyle();
                    styleRight.Alignment = new Alignment(Horizontal.Right);
                    Cell cellH2 = new Cell(RightHeader1, dt.Columns.Count, styleRight);
                    rowDesc.Cells.Add(cellH2);
                }
                if (LeftHeader2 != "" || RightHeader2 != "")
                {
                    Row rowDesc = sheet.AppendRow();
                    Cell cellH1 = new Cell(LeftHeader2, 1);
                    rowDesc.Cells.Add(cellH1);

                    Style styleRight = workbook.CreateStyle();
                    styleRight.Alignment = new Alignment(Horizontal.Right);
                    Cell cellH2 = new Cell(RightHeader2, dt.Columns.Count, styleRight);
                    rowDesc.Cells.Add(cellH2);
                }


                Style styleHeader = workbook.CreateStyle();
                styleHeader.SetBorders(1);
                styleHeader.Font = new Font() { Color = "Black", FontName = "黑体", Size = 12 };
                styleHeader.Alignment = new Alignment() { Horizontal = Horizontal.Center };


                Style styleContent = workbook.CreateStyle();
                styleContent.SetBorders(1);
                styleContent.NumberFormat.Format = NumberFormat.NumberFormatKJ;



                sheet.AppendData(styleContent, styleHeader);


                if (LeftFooter != "" || RightFooter != "")
                {
                    Row rowDesc = sheet.AppendRow();
                    Cell cellH1 = new Cell(LeftFooter, 1);
                    rowDesc.Cells.Add(cellH1);

                    Style styleRight = workbook.CreateStyle();
                    styleRight.Alignment = new Alignment(Horizontal.Right);
                    Cell cellH2 = new Cell(RightFooter, dt.Columns.Count, styleRight);
                    rowDesc.Cells.Add(cellH2);
                }
                if (actionAfter != null)
                    actionAfter(workbook, sheet);

                workbook.Save(filePath);

            }
        }
        #endregion

        public DataTable ReadTxtToList(string path, string[] cols)
        {
            //string path = @"E:\gx资料\湖南新宁中医院his\EEPNetClient\YBLog\202108304296970255834585211.txt";
            //if (!File.Exists(path))
            //{
            //    Console.WriteLine("{0} 文件不存在", path.Substring(path.LastIndexOf(@"\") + 1));
            //}
            //try
            //{
            StreamReader sr6 = File.OpenText(path);
            string text = sr6.ReadToEnd();
            sr6.Close();
            List<string> strs = Regex.Split(text, @"\r\n", RegexOptions.IgnoreCase).ToList();
            //List<JObject> strsarr = new List<JObject>();

            //List<Dictionary<string, string>> diclist = new List<Dictionary<string, string>>();
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //JObject jstr = new JObject();
            DataTable dt = new DataTable();
            for (int i = 0; i < cols.Length; i++)
            {
                dt.Columns.Add(cols[i]);
            }
            for (int i = 0; i < strs.Count; i++)
            {
                string newstr = strs[i].ToString().Replace("\t", "|");
                List<string> arrstr = newstr.Split('|').ToList();
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string val = "";
                    if (j > arrstr.Count - 1)
                    {
                        val = "";
                    }
                    else
                    {
                        val = arrstr[j];
                    }
                    if (cols[j] == "有效标志")
                    {

                    }
                    dr[j] = val;
                }
                dt.Rows.Add(dr);

            }
            return dt;
            //}
            //catch
            //{
            //    return null;
            //}
        }



        public string dms = "";
        //public List<string> lisql = new List<string>();
        public bool isxm = false;
        public string listname = "";
        public string listtype = "";
        public int sqlcount = 0;
        public FrmxzJDT jdt = new FrmxzJDT();
        public int sqlpl = 500;
        /// <summary>
        /// 分布式新增sql
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="strCloumns"></param>
        /// <param name="tabName"></param>
        /// <param name="dsCol"></param>
        /// <param name="txtList"></param>
        public int fbsAddSql(int start, int end, string[] strCloumns, string tabName, DataSet dsCol, DataTable txtList, ref string Err)
        {
            string ywCode = cbxXZZL.Text.Substring(0, 4);
            sqlcount = sqlcount + sqlpl > txtList.Rows.Count ? txtList.Rows.Count : sqlcount + sqlpl;
            int iTemp = 1;
            string sqlapp = @"insert into " + tabName + "(xzlx,";
            string valapp = ") values('" + ywCode + "',";
            if (isxm)
            {
                sqlapp = @"insert into " + tabName + "(sfxmzldm,sfxmzl,xzlx,";
                valapp = ") values('" + listtype + "','" + listname + "','" + ywCode + "',";
            }
            if (isxm)
            {
                sqlapp = @"insert into " + tabName + "(sfxmzldm,sfxmzl,xzlx,";
                valapp = ") values('" + listtype + "','" + listname + "','" + ywCode + "',";
            }
            StringBuilder insertStr = new StringBuilder();
            StringBuilder valueStr = new StringBuilder();
            valueStr.Append(valapp);
            insertStr.Append(sqlapp);
            List<string> sqlList = new List<string>();
            for (int i = start; i < end; i++)
            {
                string tablename = tabName;
                string dm = "";
                for (int m = 0; m < strCloumns.Length; m++)
                {

                    string strcol = strCloumns[m];
                    string colName = (from DataRow r in dsCol.Tables[0].Rows
                                      where r["columnName"].ToString().Equals(strcol)
                                      select r["tabColumn"].ToString()).SingleOrDefault();
                    if (string.IsNullOrEmpty(colName))
                    {
                        continue;
                    }
                    if (colName == "dm")
                    {
                        dm = txtList.Rows[i][strCloumns[m]].ToString();
                        dms += sqlcount == txtList.Rows.Count - 1 ? $"'{dm}'" : $"'{dm}',";
                    }
                    tablename = tabName;
                    if (colName == "dm")
                    {
                        dm = txtList.Rows[i][strCloumns[m]].ToString();
                        dms += i == txtList.Rows.Count - 1 ? $"'{dm}'" : $"'{dm}',";
                    }
                    //if (colName=="ybbz")
                    //{
                    //    string val = txtList.Rows[i][strCloumns[m]].ToString();
                    //    if (val!="1")
                    //    {

                    //    }
                    //}
                    insertStr.Append(colName + ",");
                    valueStr.Append("'" + txtList.Rows[i][strCloumns[m]].ToString().Replace("'", "") + "',");
                    if (iTemp <= 200)
                    {
                        iTemp++;
                    }
                    else
                    {
                        string delsql = string.Format(@"delete from {0} where dm='{1}'", tablename, dm);
                        sqlList.Add(delsql);
                        sqlList.Add(insertStr.ToString().TrimEnd(',') + valueStr.ToString().TrimEnd(',') + ")");
                        //WriteLog(insertStr.ToString().TrimEnd(',') + valueStr.ToString().TrimEnd(',') + ")");
                        insertStr.Clear();
                        valueStr.Clear();
                        insertStr.Append(sqlapp);
                        valueStr.Append(valapp);
                        iTemp = 0;
                    }
                }
                if (iTemp > 0)
                {
                    string delsql = string.Format(@"delete from {0} where dm='{1}'", tablename, dm);
                    sqlList.Add(delsql);
                    sqlList.Add(insertStr.ToString().TrimEnd(',') + valueStr.ToString().TrimEnd(',') + ")");
                    // WriteLog(insertStr.ToString().TrimEnd(',') + valueStr.ToString().TrimEnd(',') + ")");

                    insertStr.Clear();
                    valueStr.Clear();
                    insertStr.Append(sqlapp);
                    valueStr.Append(valapp);
                    iTemp = 0;

                }
            }
            object[] objs1 = sqlList.ToArray();
            object[] obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", objs1);
            int sum = txtList.Rows.Count;

            if (obj[1].ToString().Equals("1"))
            {
                this.jdt.lblJD.Text = $"目录编码为{ywCode}的字典表下载数据{start}-{end}下载成功！\r\n下载进度{sqlcount}/{sum}";
                this.jdt.pbarjd.Value = sqlcount;
                sqlList.Clear();
                return 1;
            }
            else
            {
                sqlList.Clear();
                WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "下载失败========" + obj[2].ToString());
                Err = "下载失败========" + obj[2].ToString();
                return -1;
            }

        }

        private void XZExcel()
        {
            string xmzl = cbxXZZL.Text.Substring(0, 4);
            string xmzlnm = cbxXZZL.Text;
            string[] strCloumns = null;
            string tabName = string.Empty;
            string aVer = string.Empty;
            //  
            Application.DoEvents();
            jdt = new FrmxzJDT();
            jdt.Show();
            if (cbxXZZL.SelectedIndex == 0)
            {
                tabName = "ybmrdr";
                strCloumns = new string[]{  "医疗目录编码","药品商品名","通用名编号","药品通用名","化学名称","别名","英文名称","注册名称","药监本位码","药品剂型","药品剂型名称","药品类别","药品类别名称","药品规格",
                                            "药品规格代码","注册剂型","注册规格","注册规格代码","每次用量","使用频次","酸根盐基","国家药品编号","用法","中成药标志","生产地类别","生产地类别名称","计价单位类型",
                                            "非处方药标志","非处方药标志名称","包装材质","包装材质名称","包装规格","包装数量","功能主治","给药途径","说明书","开始日期","结束日期","最小使用单位","最小销售单位",
                                            "最小计量单位","最小包装数量","最小包装单位","最小制剂单位","最小包装单位名称","最小制剂单位名称","转换比","药品有效期","最小计价单位","五笔助记码","拼音助记码",
                                            "分包装厂家","生产企业编号","生产企业名称","特殊限价药品标志","特殊药品标志","限制使用范围","限制使用标志","药品注册证号","药品注册证号开始日期","药品注册证号结束日期",
                                            "批准文号","批准文号开始日期","批准文号结束日期","市场状态","市场状态名称","药品注册批件电子档案","药品补充申请批件电子档案","国家医保药品目录备注","基本药物标志名称",
                                            "基本药物标志","增值税调整药品标志","增值税调整药品名称","上市药品目录集药品","医保谈判药品标志","医保谈判药品名称","卫健委药品编码","备注","有效标志","唯一记录号",
                                            "数据创建时间","数据更新时间","版本号","版本名称","儿童用药","公司名称","仿制药一致性评价药品","经销企业","经销企业联系人","经销企业授权书电子档案","国家医保药品目录剂型",
                                            "国家医保药品目录甲乙类标识"};
            }
            else if (cbxXZZL.SelectedIndex == 1)
            {
                tabName = "ybmrdr";
                strCloumns = new string[] { "医疗目录编码","单味药名称","单复方标志","质量等级","中草药年份","药用部位","安全计量","常规用法","性味","归经","品种","开始日期","结束日期","有效标志",
                                            "唯一记录号","数据创建时间","数据更新时间","版本号","版本名称","药材名称","功能主治","炮制方法","功效分类","药材种来源","国家医保支付政策","省级医保支付政策",
                                            "标准名称","标准页码","标准电子档案"};
            }
            else if (cbxXZZL.SelectedIndex == 2)
            {
                tabName = "ybmrdr";
                strCloumns = new string[] { "医疗目录编码","药品商品名","别名","英文名称","剂型","剂型名称","注册剂型","成分","功能主治","性状","药品规格","药品规格代码","注册规格","注册规格代码","给药途径","贮藏","使用频次",
                                            "每次用量","药品类别","药品类别名称","非处方药标志","非处方药标志名称","包装材质","包装材质名称","包装规格","说明书","包装数量","最小使用单位","最小销售单位","最小计量单位",
                                            "最小包装数量","最小包装单位","最小制剂单位","最小制剂单位名称","药品有效期","最小计价单位","不良反应","注意事项","禁忌","生产企业编号","生产企业名称","生产企业地址",
                                            "特殊限价药品标志","批准文号","批准文号开始日期","批准文号结束日期","药品注册证号","药品注册证号开始日期","药品注册证号结束日期","转换比","限制使用范围","最小包装单位名称",
                                            "注册名称","分包装厂家","市场状态","药品注册批件电子档案","药品补充申请批件电子档案","国家医保药品目录编号","国家医保药品目录备注","增值税调整药品标志","增值税调整药品名称",
                                            "上市药品目录集药品","卫健委药品编码","备注","有效标志","开始时间","结束时间","唯一记录号","数据创建时间","数据更新时间","版本号","版本名称","自制剂许可证号","儿童用药",
                                            "老年患者用药","医疗机构联系人姓名","医疗机构联系人电话","自制剂许可证电子档案"};
            }
            else if (cbxXZZL.SelectedIndex == 3)
            {
                tabName = "ybmrdr";
                strCloumns = new string[] { "医疗目录编码","计价单位","计价单位名称","诊疗项目说明","诊疗除外内容","诊疗项目内涵","有效标志","备注","服务项目类别","医疗服务项目名称","项目说明","开始日期","结束日期",
                                            "唯一记录号","版本号","版本名称" };
            }
            else if (cbxXZZL.SelectedIndex == 4)
            {
                tabName = "ybmrdr";
                strCloumns = new string[] { "医疗目录编码","耗材名称","医疗器械唯一标识码","医保通用名代码","医保通用名","产品型号","规格代码","规格","耗材分类","规格型号","材质代码","耗材材质","包装规格","包装数量","产品包装材质",
                                            "包装单位","产品转换比","最小使用单位","生产地类别","生产地类别名称","产品标准","产品有效期","性能结构与组成","适用范围","产品使用方法","产品图片编号","产品质量标准","说明书",
                                            "其他证明材料","专机专用标志","专机名称","组套名称","机套标志","限制使用标志","医保限用范围","最小销售单位","高值耗材标志","医用材料分类代码","植入材料和人体器官标志","灭菌标志",
                                            "灭菌标志名称","植入或介入类标志","植入或介入类名称","一次性使用标志","一次性使用标志名称","注册备案人名称","开始日期","结束日期","医疗器械管理类别","医疗器械管理类别名称",
                                            "注册备案号","注册备案产品名称","结构及组成","其他内容","批准日期","注册备案人住所","注册证有效期开始时间","注册证有效期结束时间","生产企业编号","生产企业名称","生产地址",
                                            "代理人企业","代理人企业地址","生产国或地区","售后服务机构","注册或备案证电子档案","产品影像","有效标志","唯一记录号","版本号","版本名称" };
            }
            else if (cbxXZZL.SelectedIndex == 5)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "西医疾病诊断ID","章","章代码范围","章名称","节代码范围","节名称","类目代码","类目名称","亚目代码","亚目名称","诊断代码","诊断名称","使用标记","国标版诊断代码","国标版诊断名称",
                                            "临床版诊断代码","临床版诊断名称","备注","有效标志","唯一记录号","数据创建时间","数据更新时间","版本号","版本名称"};
            }
            else if (cbxXZZL.SelectedIndex == 6)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "手术标准目录ID","章","章代码范围","章名称","类目代码","类目名称","亚目代码","亚目名称","细目代码","细目名称","手术操作代码","手术操作名称","使用标记","团标版手术操作代码",
                                            "团标版手术操作名称","临床版手术操作代码","临床版手术操作名称","备注","有效标志","唯一记录号","数据创建时间","数据更新时间","版本号","版本名称"};
            }
            else if (cbxXZZL.SelectedIndex == 7)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "门慢门特病种目录代码","门慢门特病种大类名称","门慢门特病种细分类名称","医保区划","备注","有效标志","唯一记录号","数据创建时间","数据更新时间","版本号","病种内涵",
                                            "版本名称","诊疗指南页码","诊疗指南电子档案","门慢门特病种名称","门慢门特病种大类代码"};
            }
            else if (cbxXZZL.SelectedIndex == 8)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "病种结算目录ID","按病种结算病种目录代码","按病种结算病种名称","限定手术操作代码","限定手术操作名称","有效标志","唯一记录号","数据创建时间","数据更新时间",
                                            "版本号","病种内涵","备注","版本名称","诊疗指南页码","诊疗指南电子档案"};
            }
            else if (cbxXZZL.SelectedIndex == 9)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "日间手术治疗目录ID","日间手术病种目录代码","日间手术病种名称","有效标志","唯一记录号","数据创建时间","数据更新时间","版本号","病种内涵","备注","版本名称",
                                            "诊疗指南页码","诊疗指南电子档案","手术操作名称","手术操作代码"};
            }
            else if (cbxXZZL.SelectedIndex == 10)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "肿瘤形态学ID", "肿瘤/细胞类型代码", "肿瘤/细胞类型", "形态学分类代码", "形态学分类", "有效标志", "唯一记录号", "数据创建时间", "数据更新时间", "版本号", "版本名称" };
            }
            else if (cbxXZZL.SelectedIndex == 11)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "中医疾病诊断ID","科别类目代码","科别类目名称","专科系统分类目代码","专科系统分类目名称","疾病分类代码","疾病分类名称","备注","有效标志","唯一记录号",
                                            "数据创建时间","数据更新时间","版本号","版本名称"};
            }
            else if (cbxXZZL.SelectedIndex == 12)
            {
                tabName = "ybbzmrdr";
                strCloumns = new string[] { "中医证候ID","证候类目代码","证候类目名称","证候属性代码","证候属性","证候分类代码","证候分类名称","备注","有效标志","唯一记录号",
                                            "数据创建时间","数据更新时间","版本号","版本名称"};
            }
            aVer = "0";
            string strBBH = string.Format(@" select top 1 max(id) id,bbmc from {0} where xzlx='{1}' group by bbmc order by id desc ", tabName, xmzl);
            DataSet dsBBH = CliUtils.ExecuteSql("sybdj", "cmd", strBBH, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsBBH.Tables[0].Rows.Count > 0)
            {
                aVer = dsBBH.Tables[0].Rows[0]["bbmc"].ToString();
            }
            else
            {
                if (xmzl.Equals("1305"))
                {
                    aVer = "F002_19970101000000_C";
                }
                else
                {
                    aVer = "0";
                }
            }
            if (!string.IsNullOrEmpty(txtbbh.Text.ToString()))
                aVer = txtbbh.Text.ToString();
            string ywCode = cbxXZZL.Text.Substring(0, 4);

            object[] obj = { ywCode, aVer };
            obj = yb_interface_hn_nkNew.YBMLXZ(obj);
            if (chbxzyw.Checked)
                return;
            if (obj[1].ToString() == "1")
            {
                string path = obj[2].ToString();
                path = "YBLog\\" + path.Substring(0, path.Length - 4);
                string sPath = Path.Combine(Environment.CurrentDirectory, path);

                if (!File.Exists(sPath))
                {
                    MessageBox.Show("文件不存在！");
                    return;
                }
                //DataTable dt = ExcelFunc.GetDataTableFromFile(path, strCloumns, "制表符", "gb2312");
                DataTable txtList = ReadTxtToList(sPath, strCloumns);
                if (txtList == null)
                    return;
                int code = 1;
                string Err = "";

                this.jdt.pbarjd.Maximum = txtList.Rows.Count;
                this.jdt.pbarjd.Value = 0;
                jdt.lblJD.Text = "下载开始！";
                if (txtList.Rows.Count > 0)
                {
                    gridView.DataSource = txtList;
                    string strSql = string.Empty;
                    string colName = string.Empty;
                    StringBuilder insertStr = new StringBuilder();
                    StringBuilder valueStr = new StringBuilder();

                    strSql = string.Format(@" select tabColumn,columnName from colCompare where tableName='{0}'   and beLong='{1}'  ", tabName, cbxXZZL.Text.Substring(0, 4));
                    DataSet dsCol = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    bool isxm = this.getlisttypeByYwcode(ywCode, ref listname, ref listtype);
                    int zsl = txtList.Rows.Count % sqlpl > 0 ? (txtList.Rows.Count / sqlpl) + 1 : (txtList.Rows.Count / sqlpl);
                    for (int i = 0; i < zsl; i++)
                    {
                        int start = i * sqlpl;
                        int end = (i + 1) * sqlpl > txtList.Rows.Count ? txtList.Rows.Count : (i + 1) * sqlpl;
                        Task t1 = Task.Factory.StartNew(() =>
                        {
                            lock (this)
                            {
                                code = fbsAddSql(start, end, strCloumns, tabName, dsCol, txtList, ref Err);
                            }
                        });
                        t1.Wait();
                        if (code == -1)
                        {
                            break;
                        }

                    }
                    #region 合并下载
                    //for (int i = 0; i < cs; i++)
                    //{
                    //    int start = i * plxzsl;
                    //    int end = (i + 1) * plxzsl > sqls.Length ? sqls.Length : (i + 1) * plxzsl;
                    //    List<string> plsql = new List<string>();
                    //    for (int j = start; j < end; j++)
                    //    {
                    //        plsql.Add(sqls[j].ToString());
                    //    }
                    //    object[] objs1 = plsql.ToArray();
                    //    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", objs1);
                    //    if (obj[1].ToString().Equals("1"))
                    //    {
                    //        plsql.Clear();
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "下载失败========" + obj[2].ToString());
                    //        code = -1;
                    //        Err = "下载失败========" + obj[2].ToString();
                    //        break;
                    //    }

                    //} 
                    #endregion
                    if (code < 0)
                    {
                        MessageBox.Show(Err, "错误提示");
                        this.jdt.Close();
                        sqlcount = 0;
                        return;
                    }
                    strBBH = string.Format(@"update {0} set  pym = dbo.f_get_PY(dmmc,10),wbm=dbo.f_get_WB(dmmc,10) where xzlx='{1}'", tabName, ywCode);//, xmzl, dms
                    WriteLog("\r\n修改语句：" + strBBH);
                    dsBBH = CliUtils.ExecuteSql("sybdj", "cmd", strBBH, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (isxm)
                    {
                        strBBH = string.Format(@" update {0} set sfxmzldm='{1}',sfxmzl='{2}' where (isnull(sfxmzldm,'')!='{1}' or isnull(sfxmzl,'')!='{2}') and xzlx='{3}'", tabName, listtype, listname, ywCode);

                        WriteLog("\r\n修改语句修改listtype：" + strBBH);
                        dsBBH = CliUtils.ExecuteSql("sybdj", "cmd", strBBH, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    }

                    if (ywCode == "1309")
                    {
                        strBBH = string.Format(@" update {0} set  yllb='14',bzlb='门诊慢性病' where   xzlx='{1}'", tabName, ywCode);

                        WriteLog("\r\n修改语句修改病种：" + strBBH);
                        dsBBH = CliUtils.ExecuteSql("sybdj", "cmd", strBBH, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    }

                    if (ywCode == "1307")
                    {
                        strBBH = string.Format(@" update {0} set  yllb='11',bzlb='普通病种',ybbz='2',bz='1' where   xzlx='{1}'", tabName, ywCode);

                        WriteLog("\r\n修改语句修改病种：" + strBBH);
                        dsBBH = CliUtils.ExecuteSql("sybdj", "cmd", strBBH, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    }
                    MessageBox.Show("数据下载成功！");
                    sqlcount = 0;
                    this.jdt.Close();
                }
            }
            else
            {
                WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "下载失败========" + obj[2].ToString());
                MessageBox.Show(obj[2].ToString());
                sqlcount = 0;
                this.jdt.Close();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XZExcel();
        }

        private void btncz_Click(object sender, EventArgs e)
        {
        }

        private void btnxz2_Click(object sender, EventArgs e)
        {
            string aVer = "";
            aVer = "20200101";
            int cshSum = 10000;
            int cshcount = 10;
            int xzcount = 0;
            int plcount = 500;
            int status = 1;
            string YwCode = this.cbxXZZL.Text.Substring(0, 4);
            object[] obj = { YwCode, aVer, "1", plcount.ToString() };
            this.jdt = new FrmxzJDT();
            Application.DoEvents();
            jdt.Show();
            this.jdt.lblJD.Text = "下载开始";
            List<JObject> jlist = new List<JObject>();
            for (int i = 0; i < cshcount; i++)
            {

                ThreadStart ts = new ThreadStart(() =>
                {
                    obj = new object[] { YwCode, aVer, (i + 1).ToString(), plcount.ToString() };
                    obj = DownLoadTwo(obj);
                });
                ts.Invoke();
                if (obj[1].ToString() == "0")
                {
                    status = -1;
                    break;
                }
                int sum = (int)obj[3];
                List<JObject> pljlist = obj[2] as List<JObject>;
                jlist.AddRange(pljlist);
                gridView.DataSource = jlist;
                if (sum != cshSum)
                {
                    cshSum = sum;
                    cshcount = cshSum % plcount == 0 ? cshSum / plcount : (cshSum / plcount) + 1;
                }
                xzcount = jlist.Count;
                Task.Factory.StartNew(() =>
                {
                    this.jdt.pbarjd.Maximum = cshSum;
                    this.jdt.pbarjd.Value = xzcount;
                    this.jdt.lblJD.Text = $"下载业务{YwCode}数据下载成功！下载进度{xzcount}/{cshSum}￣へ￣";
                }).Wait();
            }
            if (status != -1)
            {
                MessageBox.Show($"下载业务{YwCode}数据全部下载成功！\r\n下载条数{cshSum}!(*^▽^*)", "提示");
                this.jdt.Close();
            }
            else
            {
                MessageBox.Show($"当前下载{xzcount}条,由于医保返回错误终止下载!o(╥﹏╥)o" + obj[2].ToString(), "提示");
                this.jdt.Close();
            }

        }

        /// <summary>
        /// 下载2调用的方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object[] DownLoadTwo(object[] obj)
        {
            switch (cbxXZZL.Text.Substring(0, 4))
            {
                case "1304":
                    obj = yb_interface_hn_nkNew.MZYPMLCX(obj);
                    break;
                case "1312":
                    obj = yb_interface_hn_nkNew.YBMLXXXZ(obj);
                    break;
                case "1316":
                    obj = yb_interface_hn_nkNew.YLMLYYBMLPPXXXZ(obj);
                    break;
                case "1317":
                    obj = yb_interface_hn_nkNew.YYJGMLPPXXCX(obj);
                    break;
                case "1318":
                    obj = yb_interface_hn_nkNew.YBMLXJXXXZ(obj);
                    break;
                case "1319":
                    obj = yb_interface_hn_nkNew.YBMLXZFBLXXCX(obj);
                    break;
                default:
                    MessageBox.Show("选择的下载类型不支持该接口！");
                    break;
            }

            return obj;
        }


        public void WriteLog(string str)
        {
            if (!Directory.Exists("YBLog\\YBSql"))
            {
                Directory.CreateDirectory("YBLog\\YBSql");
            }
            FileStream stream = new FileStream("YBLog\\YBSql\\YBSql" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Close();
            stream.Close();
        }

        public bool getlisttypeByYwcode(string ywCode, ref string listName, ref string listtype)
        {
            switch (ywCode)
            {
                case "1301":
                    listName = list_type.西药中成药.ToString();
                    listtype = ((int)list_type.西药中成药).ToString();
                    return true;
                case "1302":
                    listName = list_type.中药饮片.ToString();
                    listtype = ((int)list_type.中药饮片).ToString();
                    return true;
                case "1305":
                    listName = list_type.医疗服务项目.ToString();
                    listtype = ((int)list_type.医疗服务项目).ToString();
                    return true;
                case "1303":
                    listName = list_type.自制剂.ToString();
                    listtype = ((int)list_type.自制剂).ToString();
                    return true;
                case "1306":
                    listName = list_type.医用耗材.ToString();
                    listtype = ((int)list_type.医用耗材).ToString();
                    return true;
                default:
                    return false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string aVer = "20200101";
            string type = this.txtzdType.Text;
            string parentValue = this.txtZDkey.Text;
            string date = this.dtqueryDate.Value.ToString("yyyy-MM-dd");
            //if (string.IsNullOrEmpty(type))
            //{
            //    MessageBox.Show("字典类型不为空！");
            //    return;
            //}
            //if (string.IsNullOrEmpty(parentValue))
            //{
            //    MessageBox.Show("父字典键值不得为空！");
            //    return;
            //}
            //if (string.IsNullOrEmpty(date))
            //{
            //    MessageBox.Show("查询时间不得为空！");
            //    return;
            //}
            object[] obzd = { type, parentValue, date };
            obzd = yb_interface_hn_nkNew.YBZDBCX(obzd);
            if (obzd[1].ToString() == "1")
            {
                List<JObject> jlist = (List<JObject>)obzd[2];
                if (jlist.Count > 0)
                {
                    gridView.DataSource = jlist;
                    MessageBox.Show("字典表查询成功");
                }
                else
                {
                    MessageBox.Show("字典表查询无数据");
                }
            }
            else
            {
                MessageBox.Show(obzd[2].ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ywCode = cbxXZZL.SelectedItem.ToString().Substring(0, 4);
            switch (ywCode)
            {
                case "3301":
                    break;
                case "3302":

                default:
                    break;
            }
        }
    }
}
