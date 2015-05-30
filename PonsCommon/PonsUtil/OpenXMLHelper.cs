using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace PonsUtil
{

    /// <summary>
    /// 操作模式
    /// </summary>
    public enum OpenExcelMode
    {
        /// <summary>
        /// 创建新excel
        /// </summary>
        CreateNew = 1,
        /// <summary>
        /// 只读打开
        /// </summary>
        OpenForRead = 2,
        /// <summary>
        /// 编辑打开
        /// </summary>
        OpenForEdit = 3
    }
    public class OpenXMLHelper : IDisposable
    {
        #region 私有属性
        private SpreadsheetDocument _document = null;
        private bool _isEidtable = false;

        private SharedStringTablePart _ssPart = null;
        private SharedStringTablePart _shareStringPart
        {
            get
            {
                if (_ssPart == null)
                {
                    _ssPart = _document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                    if (_ssPart == null)
                    {
                        _ssPart = _document.WorkbookPart.AddNewPart<SharedStringTablePart>();
                    }
                }
                return _ssPart;
            }
        }
        #endregion

        #region 公有属性
        /// <summary>
        /// 是否自动保存
        /// </summary>
        public bool IsAutoSave { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <param name="mode">打开模式</param>
        public OpenXMLHelper(string filePath, OpenExcelMode mode)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("没有配置文件全路径");
            }
            switch (mode)
            {
                case OpenExcelMode.CreateNew:
                    _document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
                    WorkbookPart workbookpart = _document.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();
                    _isEidtable = true;
                    break;
                case OpenExcelMode.OpenForEdit:
                    _document = SpreadsheetDocument.Open(filePath, true);
                    _isEidtable = true;
                    break;
                case OpenExcelMode.OpenForRead:
                    _document = SpreadsheetDocument.Open(filePath, false);
                    _isEidtable = false;
                    break;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">IO流</param>
        /// <param name="mode">打开模式</param>
        public OpenXMLHelper(Stream stream, OpenExcelMode mode)
        {
            if (stream == null)
            {
                throw new ArgumentException("没有配置流");
            }
            switch (mode)
            {
                case OpenExcelMode.CreateNew:
                    _document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
                    WorkbookPart workbookpart = _document.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();
                    _isEidtable = true;
                    break;
                case OpenExcelMode.OpenForEdit:
                    _document = SpreadsheetDocument.Open(stream, true);
                    _isEidtable = true;
                    break;
                case OpenExcelMode.OpenForRead:
                    _document = SpreadsheetDocument.Open(stream, false);
                    _isEidtable = false;
                    break;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~OpenXMLHelper()
        {
            Dispose();
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 读取指定工作表的数据
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns>数据</returns>
        public string[,] ReadUsedRangeToEnd(string sheetName = null)
        {
            Worksheet worksheet = GetWorksheetByName(sheetName);
            var rows = worksheet.Descendants<Row>();
            int rowCount = rows.Count();
            int columnCount = 0;
            int ri = 0;
            int ci = 0;
            foreach (Row row in rows)
            {
                if (TryGetRowIndexAndColumnIndex(row.Descendants<Cell>().Last().CellReference.Value, out ri, out ci))
                {
                    if (ci > columnCount)
                    {
                        columnCount = ci;
                    }
                }
            }
            string[,] data = new string[rowCount, columnCount];
            int x = 0;
            foreach (Row row in rows)
            {
                foreach (Cell cell in row)
                {
                    if (TryGetRowIndexAndColumnIndex(cell.CellReference.Value, out ri, out ci))
                    {
                        data[x, ci - 1] = GetValue(cell);
                    }
                }
                x++;
            }
            return data;
        }

        /// <summary>
        /// 读取指定工作表的数据(不带末尾的空行空列)
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns>数据</returns>
        public string[,] ReadUsedRangeToEndWithoutBlank(string sheetName = null)
        {
            string[,] data = ReadUsedRangeToEnd(sheetName);
            int row = data.GetLength(0);
            int column = data.GetLength(1);
            bool isBlank = true;
            //排除空行
            for (int i = data.GetLength(0) - 1; i >= 0; i--)
            {
                isBlank = true;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j] != null && !string.IsNullOrWhiteSpace(data[i, j].ToString()))
                    {
                        isBlank = false;
                        break;
                    }
                }
                if (isBlank)
                {
                    row = row - 1;
                }
            }
            if (row <= 0)
            {
                return null;
            }
            //排除空列
            for (int i = data.GetLength(1) - 1; i >= 0; i--)
            {
                isBlank = true;
                for (int j = 0; j < row; j++)
                {
                    if (data[j, i] != null && !string.IsNullOrWhiteSpace(data[j, i].ToString()))
                    {
                        isBlank = false;
                        break;
                    }
                }
                if (isBlank)
                {
                    column = column - 1;
                }
            }
            if (column <= 0)
            {
                return null;
            }
            string[,] newData = new string[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    newData[i, j] = data[i, j];
                }
            }
            return newData;
        }

        /// <summary>
        /// 创建新工作表
        /// </summary>
        /// <param name="sheetName">工作表名(默认为sheet+数字)</param>
        /// <returns>是否成功</returns>
        public bool CreateNewWorksheet(string sheetName = null)
        {
            if (!_isEidtable)
            {
                return false;
            }
            //创建一个新的WorkssheetPart（后面将用它来容纳具体的Sheet）
            WorksheetPart worksheetPart = _document.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            //取得Sheet集合
            Sheets sheets = _document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            if (sheets == null)
            {
                sheets = _document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            }
            string relationshipId = _document.WorkbookPart.GetIdOfPart(worksheetPart);
            //得到Sheet的唯一序号
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }
            string sheetTempName = "Sheet" + sheetId;
            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                bool hasSameName = false;
                //检测是否有重名
                foreach (var item in sheets.Elements<Sheet>())
                {
                    if (item.Name == sheetName)
                    {
                        hasSameName = true;
                        break;
                    }
                }
                if (!hasSameName)
                {
                    sheetTempName = sheetName;
                }
            }
            //创建Sheet实例并将它与sheets关联
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetTempName };
            sheets.Append(sheet);
            return true;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (!_isEidtable)
            {
                return;
            }
            _document.WorkbookPart.Workbook.Save();
        }

        /// <summary>
        /// 设置区域字体颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="startRowIndex">左上角行号(1开始)</param>
        /// <param name="startColIndex">左上角列号(1开始)</param>
        /// <param name="endRowIndex">右下角行号(1开始)</param>
        /// <param name="endColIndex">右下角列号(1开始)</param>
        /// <param name="sheetName">工作表名</param>
        /// <returns>是否成功</returns>
        public bool SetRangeFontColor(System.Drawing.Color color, int startRowIndex, int startColIndex, int endRowIndex, int endColIndex, string sheetName = null)
        {
            if (!_isEidtable)
            {
                return false;
            }
            if (startRowIndex < 0
                || startRowIndex > endRowIndex
                || startColIndex < 0
                || startColIndex > endColIndex
                || endRowIndex < 0
                || endColIndex < 0)
            {
                return false;
            }

            CreateDefaultStyleSheet();
            Stylesheet stylesheet = _document.WorkbookPart.WorkbookStylesPart.Stylesheet;

            HexBinaryValue fontColor = ConvertColor(color);

            Font newFont = new Font();
            newFont.Color = new Color();
            newFont.Color.Rgb = fontColor;
            bool isFontAppended = false;

            CellFormat newCellFormat = new CellFormat();
            newCellFormat.FormatId = 0;
            newCellFormat.ApplyFont = true;
            bool isCellFormatAppended = false;

            Worksheet worksheet = GetWorksheetByName(sheetName);
            var rows = worksheet.Descendants<Row>().Where(r => (r.RowIndex >= startRowIndex && r.RowIndex <= endRowIndex));
            int ri = 0;
            int ci = 0;
            foreach (Row row in rows)
            {
                foreach (Cell cell in row)
                {
                    if (TryGetRowIndexAndColumnIndex(cell.CellReference.Value, out ri, out ci))
                    {
                        if (ci < startColIndex)
                        {
                            continue;
                        }
                        if (ci > endColIndex)
                        {
                            break;
                        }
                    }
                    if (cell.StyleIndex != null && cell.StyleIndex > 0)
                    {
                        CellFormat cf = (CellFormat)stylesheet.CellFormats.ChildElements.ElementAtOrDefault((int)cell.StyleIndex.Value);
                        if (cf.FontId != null && cf.FontId > 0)
                        {
                            Font f = (Font)stylesheet.Fonts.ChildElements.ElementAtOrDefault((int)cf.FontId.Value);
                            if (f.Color == null)
                            {
                                f.Color = new Color();
                            }
                            f.Color.Rgb = fontColor;
                        }
                        else
                        {
                            if (!isFontAppended)
                            {
                                isFontAppended = true;
                                stylesheet.Fonts.Append(newFont);
                            }
                            cf.FontId = (uint)stylesheet.Fonts.ChildElements.Count - 1;
                        }
                    }
                    else
                    {
                        if (!isFontAppended)
                        {
                            isFontAppended = true;
                            stylesheet.Fonts.AppendChild(newFont);
                            stylesheet.Fonts.Count++;
                        }
                        if (!isCellFormatAppended)
                        {
                            isCellFormatAppended = true;
                            newCellFormat.FontId = (uint)stylesheet.Fonts.ChildElements.Count - 1;
                            stylesheet.CellFormats.AppendChild(newCellFormat);
                            stylesheet.CellFormats.Count++;
                        }
                        cell.StyleIndex = (uint)stylesheet.CellFormats.ChildElements.Count - 1;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 向指定工作表写数据(从最左上角开始写)
        /// </summary>
        /// <param name="data">具体数据</param>
        /// <param name="sheetName">工作表名</param>
        /// <returns>是否成功</returns>
        public bool WriteData(string[,] data, string sheetName = null)
        {
            if (!_isEidtable)
            {
                return false;
            }
            if (data == null)
            {
                return false;
            }
            return this.Write(data, 1, 1, sheetName);
        }

        /// <summary>
        /// 向指定工作表指定位置写数据
        /// </summary>
        /// <param name="data">具体数据</param>
        /// <param name="startRowIndex">写入开始行号(从1开始)</param>
        /// <param name="startColIndex">写入开始列号(从1开始)</param>
        /// <param name="sheetName">工作表名</param>
        /// <returns>是否成功</returns>
        public bool WriteData(string[,] data, int startRowIndex, int startColIndex, string sheetName = null)
        {
            if (!_isEidtable)
            {
                return false;
            }
            if (data == null)
            {
                return false;
            }
            int maxRowCount = data.GetLength(0);
            int maxColCount = data.GetLength(1);
            if (startRowIndex < 1 || startRowIndex > maxRowCount)
            {
                startRowIndex = 1;
            }
            if (startColIndex < 1 || startColIndex > maxColCount)
            {
                startColIndex = 1;
            }
            return this.Write(data, startRowIndex, startColIndex, sheetName);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得表格内容
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private string GetValue(Cell cell)
        {
            if (cell.ChildElements.Count == 0)
                return null;
            //get cell value
            double d;
            String value = "";
            if (double.TryParse(cell.CellValue.InnerText, out d))
            {
                value = d.ToString();
            }
            else
            {
                value = cell.CellValue.InnerText;
            }
            //Look up real value from shared string table
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                value = _document.WorkbookPart.SharedStringTablePart.SharedStringTable
                .ChildElements[Int32.Parse(value)]
                .InnerText;
            return value;
        }

        /// <summary>
        /// 转换Drawing颜色为openxml的颜色
        /// </summary>
        /// <param name="color">Drawing颜色</param>
        /// <returns>openxml的rgb颜色</returns>
        private HexBinaryValue ConvertColor(System.Drawing.Color color)
        {
            HexBinaryValue value = new HexBinaryValue();
            value.Value = System.Drawing.ColorTranslator.ToHtml(
                        System.Drawing.Color.FromArgb(
                            color.A,
                            color.R,
                            color.G,
                            color.B)).Replace("#", "");
            return value;
        }

        /// <summary>
        /// 添加默认的样式
        /// </summary>
        private void CreateDefaultStyleSheet()
        {
            WorkbookStylesPart workbookStylesPart = null;
            if (_document.WorkbookPart.GetPartsCountOfType<WorkbookStylesPart>() == 0)
            {
                workbookStylesPart = _document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            }
            else
            {
                workbookStylesPart = _document.WorkbookPart.GetPartsOfType<WorkbookStylesPart>().FirstOrDefault();
            }
            if (workbookStylesPart.Stylesheet == null)
            {
                workbookStylesPart.Stylesheet = new Stylesheet();
            }
            if (workbookStylesPart.Stylesheet.Fonts == null)
            {
                Fonts fonts = new Fonts();
                fonts.KnownFonts = true;
                fonts.Count = 1;
                Font defaultFont = new Font();
                fonts.Append(defaultFont);
                workbookStylesPart.Stylesheet.Append(fonts);
            }
            if (workbookStylesPart.Stylesheet.CellFormats == null)
            {
                CellFormats cellFormats = new CellFormats();
                cellFormats.Count = 1;
                CellFormat defaultCellFormat = new CellFormat();
                defaultCellFormat.FontId = 0;
                cellFormats.Append(defaultCellFormat);
                workbookStylesPart.Stylesheet.Append(cellFormats);
            }
        }

        /// <summary>
        /// 根据工作表名取得工作表
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns>工作表</returns>
        private Worksheet GetWorksheetByName(string sheetName = null)
        {
            Sheet sheet = null;
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                sheet = _document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
            }
            else
            {
                sheet = _document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
            }
            if (sheet == null)
            {
                return null;
            }
            WorksheetPart worksheetPart = (WorksheetPart)_document.WorkbookPart.GetPartById(sheet.Id);
            return worksheetPart.Worksheet;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="startRowIndex">开始行号</param>
        /// <param name="startColIndex">开始列号</param>
        /// <param name="sheetName">工作表名</param>
        /// <returns>是否成功</returns>
        private bool Write(string[,] data, int startRowIndex, int startColIndex, string sheetName)
        {
            Worksheet worksheet = GetWorksheetByName(sheetName);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    AppendCellData(data[i, j], i + startRowIndex, j + startColIndex, worksheet);
                }
            }
            return true;
        }

        /// <summary>
        /// 往表格添加数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="rowIndex">行号</param>
        /// <param name="colIndex">列号</param>
        /// <param name="worksheet">工作表</param>
        private void AppendCellData(string data, int rowIndex, int colIndex, Worksheet worksheet)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            Cell cell = InsertCellInWorksheet(rowIndex, colIndex, worksheet);
            cell.DataType = CellValues.SharedString;
            cell.CellValue = new CellValue(InsertSharedStringItem(data).ToString());
        }

        /// <summary>
        /// 获得excel的位置字母(如A2,B5)
        /// </summary>
        /// <param name="rowIndex">行号</param>
        /// <param name="colIndex">列号</param>
        /// <returns>列字母</returns>
        private string GetExcelRowColumn(int rowIndex, int colIndex)
        {
            return Number2String(colIndex) + rowIndex.ToString();
        }

        /// <summary>
        /// 根据excel的行列号取得对应的行号和列号
        /// </summary>
        /// <param name="rowColString">excel的行列号，如A1,AB1</param>
        /// <param name="rowIndex">行号</param>
        /// <param name="colIndex">列号</param>
        /// <returns>是否能转换</returns>
        private bool TryGetRowIndexAndColumnIndex(string rowColString, out int rowIndex, out int colIndex)
        {
            rowIndex = 0;
            colIndex = 0;
            if (string.IsNullOrWhiteSpace(rowColString))
            {
                return false;
            }
            string rowStr = "";
            string colStr = "";
            foreach (char c in rowColString)
            {
                if (char.IsNumber(c))
                {
                    if (string.IsNullOrWhiteSpace(colStr))
                    {
                        return false;
                    }
                    rowStr += c;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(rowStr))
                    {
                        return false;
                    }
                    colStr += c;
                }
            }
            colIndex = String2Number(colStr);
            if (colIndex <= 0)
            {
                return false;
            }
            rowIndex = int.Parse(rowStr);
            return true;
        }

        /// <summary>
        /// 根据excel列字母获得数字
        /// </summary>
        /// <param name="s">列字母</param>
        /// <returns>数字</returns>
        private int String2Number(string s)
        {
            int r = 0;
            for (int i = 0; i < s.Length; i++)
            {
                r = r * 26 + s[i] - 'A' + 1;
            }
            return r;
        }

        /// <summary>
        /// 把数字转换为列所对应的字母
        /// </summary>
        /// <param name="n">数字</param>
        /// <returns>字母</returns>
        private string Number2String(int n)
        {
            string s = "";     // result  
            int r = 0;         // remainder  

            while (n != 0)
            {
                r = n % 26;
                char ch = ' ';
                if (r == 0)
                    ch = 'Z';
                else
                    ch = (char)(r - 1 + 'A');
                s = ch.ToString() + s;
                if (s[0] == 'Z')
                    n = n / 26 - 1;
                else
                    n /= 26;
            }
            return s;
        }

        /// <summary>
        /// 插入表格到工作表
        /// </summary>
        /// <param name="rowIndex">行号(1开始)</param>
        /// <param name="colIndex">列号(1开始)</param>
        /// <param name="worksheet">工作表</param>
        /// <returns>表格</returns>
        private Cell InsertCellInWorksheet(int rowIndex, int colIndex, Worksheet worksheet)
        {
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = GetExcelRowColumn(rowIndex, colIndex);

            //如果指定的行存在，则直接返回该行，否则插入新行
            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row() { RowIndex = Convert.ToUInt32(rowIndex) };
                sheetData.Append(row);
            }

            //如果该行没有指定ColumnName的列，则插入新列，否则直接返回该列
            Cell returnCell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value.ToUpper() == cellReference.ToUpper());
            if (returnCell != null)
            {
                return returnCell;
            }
            else
            {
                //列必须按(字母)顺序插入，因此要先根据"列引用字符串"查找插入的位置
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    int ri = 0;
                    int ci = 0;
                    if (TryGetRowIndexAndColumnIndex(cell.CellReference.Value, out ri, out ci))
                    {
                        if (ci > colIndex)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }
                returnCell = new Cell() { CellReference = cellReference };
                if (refCell == null)
                {
                    row.Append(returnCell);
                }
                else
                {
                    row.InsertBefore(returnCell, refCell);
                }
                return returnCell;
            }
        }

        /// <summary>
        /// 插入共享字符表
        /// </summary>
        /// <param name="data">字符</param>
        /// <returns>共享字符表中的序号</returns>
        private int InsertSharedStringItem(string data)
        {
            SharedStringTable sharedStringTable = _shareStringPart.SharedStringTable;
            if (sharedStringTable == null)
            {
                sharedStringTable = new SharedStringTable();
                _shareStringPart.SharedStringTable = sharedStringTable;
            }
            int i = 0;
            //遍历SharedStringTable中所有的Elements，查看目标字符串是否存在
            foreach (SharedStringItem item in sharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == data)
                {
                    return i;
                }
                i++;
            }
            //如果目标字符串不存在，则创建一个，同时把SharedStringTable的最后一个Elements的"索引+1"返回
            sharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(data)));
            return i;
        }
        #endregion

        #region IDisposable 成员
        public void Dispose()
        {
            if (_document != null)
            {
                if (IsAutoSave)
                {
                    Save();
                }
                _document.Dispose();
            }
        }

        #endregion
    }
}
