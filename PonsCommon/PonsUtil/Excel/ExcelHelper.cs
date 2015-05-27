using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;
using PonsUtil.Export;

namespace PonsUtil.Excel
{
    public class ExcelHelper
    {
        /// <summary>        
        /// 导出列名       
        /// </summary>        
        public static SortedList ListColumnsName;

        /// <summary>
        /// DataTable导出到Excel文件(不含表头) 
        /// </summary>
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strFileName">保存位置</param>
        public static void Export(DataTable dtSource, string strFileName)
        {
            Export(dtSource, false, string.Empty, strFileName, string.Empty, null);
        }

        /// <summary>
        /// DataTable导出到Excel文件(不含表头) 
        /// </summary>
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strFileName">保存位置</param>
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        public static void Export(DataTable dtSource, string strFileName, string errorColumn)
        {
            Export(dtSource, false, string.Empty, strFileName, errorColumn, null);
        }

        /// <summary>
        /// DataTable导出到Excel文件(不含表头) 
        /// </summary>
        /// <param name="dsSource">源DataTable</param>   
        /// <param name="strFileName">保存位置</param>
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        public static void Export(DataSet dsSource, string strFileName, string errorColumn)
        {
            Export(dsSource, false, string.Empty, strFileName, errorColumn, null);
        }

        /// <summary>
        /// DataTable导出到Excel文件(不含表头) 
        /// </summary>
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strFileName">保存位置</param>
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        /// <param name="callBack">每个单元格的回调</param>
        public static void Export(DataTable dtSource, string strFileName, string errorColumn,
                                  Action<DataRow, HSSFWorkbook, HSSFSheet, HSSFCell> callBack)
        {
            Export(dtSource, false, string.Empty, strFileName, errorColumn, callBack);
        }

        /// <summary>   
        /// DataTable导出到Excel文件   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="hasHeader">是否设置表头</param>
        /// <param name="strHeaderText">表头文本</param>   
        /// <param name="strFileName">保存位置</param>
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        public static void Export(DataTable dtSource, bool hasHeader, string strHeaderText, string strFileName,
                                  string errorColumn, Action<DataRow, HSSFWorkbook, HSSFSheet, HSSFCell> callBack)
        {
            using (MemoryStream ms = Export(dtSource, hasHeader, strHeaderText, errorColumn, callBack))
            {
                using (var fs = new FileStream(strFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }

            using (FileStream s = File.Open(strFileName, FileMode.Open))
            {
                string serverDirName = string.Format(@"{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month,
                                                     DateTime.Now.Day, "ErrorFiles");
                var c = new FtpFileOperationControl();
                c.UploadFileStream = s;
                c.UploadFileName = Path.Combine(serverDirName, Path.GetFileName(strFileName));
                c.MakeDirName = serverDirName;
                c.Do(FtpFileAction.Upload);
            }
        }


        /// <summary>   
        /// DataSet导出到Excel文件   
        /// </summary>   
        /// <param name="dsSource">源dsSource</param>   
        /// <param name="hasHeader">是否设置表头</param>
        /// <param name="strHeaderText">表头文本</param>   
        /// <param name="strFileName">保存位置</param>
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        public static void Export(DataSet dsSource, bool hasHeader, string strHeaderText, string strFileName,
                                  string errorColumn, Action<DataRow, HSSFWorkbook, HSSFSheet, HSSFCell> callBack)
        {
            using (MemoryStream ms = Export(dsSource, hasHeader, strHeaderText, errorColumn, callBack))
            {
                using (var fs = new FileStream(strFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }

            using (FileStream s = File.Open(strFileName, FileMode.Open))
            {
                string serverDirName = string.Format(@"{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month,
                                                     DateTime.Now.Day, "ErrorFiles");
                var c = new FtpFileOperationControl();
                c.UploadFileStream = s;
                c.UploadFileName = Path.Combine(serverDirName, Path.GetFileName(strFileName));
                c.MakeDirName = serverDirName;
                c.Do(FtpFileAction.Upload);
            }
        }


        /// <summary>
        /// DataTable导出到Excel的MemoryStream   
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="hasHeader"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="errorColumn"></param>
        /// <returns></returns>
        public static MemoryStream Export(DataTable dtSource, bool hasHeader, string strHeaderText, string errorColumn)
        {
            return Export(dtSource, hasHeader, strHeaderText, errorColumn, null);
        }

        /// <summary>   
        /// DataTable导出到Excel的MemoryStream   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>
        /// <param name="hasHeader">是否设置表头</param>
        /// <param name="strHeaderText">表头文本</param> 
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        /// <param name="callBack">每个单元格的回调</param>
        /// <returns></returns>
        public static MemoryStream Export(DataTable dtSource, bool hasHeader, string strHeaderText, string errorColumn,
                                          Action<DataRow, HSSFWorkbook, HSSFSheet, HSSFCell> callBack)
        {
            var workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet();

            #region 右击文件 属性信息

            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                workbook.SummaryInformation = si;
            }

            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle();
            HSSFDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

            HSSFCellStyle errorStyle = workbook.CreateCellStyle();
            HSSFFont errorFont = workbook.CreateFont();
            errorFont.Color = HSSFColor.RED.index;
            errorStyle.SetFont(errorFont);

            //取得列宽   
            var arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    #region 表头及样式

                    if (hasHeader)
                    {
                        HSSFRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                        HSSFFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                    }

                    #endregion

                    #region 列头及样式

                    {
                        int rowNum = hasHeader ? 1 : 0;
                        HSSFRow headerRow = sheet.CreateRow(rowNum);
                        HSSFCellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                        HSSFFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽   
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                        rowIndex = rowNum + 1;
                    }

                    #endregion
                }

                #endregion

                #region 填充内容

                HSSFRow dataRow = sheet.CreateRow(rowIndex);
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    if (dtSource.Columns[i].ColumnName == errorColumn)
                    {
                        newCell.CellStyle = errorStyle;
                    }
                    if (callBack != null)
                    {
                        callBack(row, workbook, sheet, newCell);
                    }
                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime": //日期类型   
                            DateTime dateV;
                            if (!String.IsNullOrEmpty(drValue))
                            {
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);
                                newCell.CellStyle = dateStyle; //格式化显示 
                            }
                            else
                                newCell.SetCellValue("");
                            break;
                        case "System.Boolean": //布尔型   
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型   
                        case "System.Int32":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Int64":
                            long longV = 0;
                            long.TryParse(drValue, out longV);
                            newCell.SetCellValue(longV);
                            break;
                        case "System.Decimal": //浮点型   
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DBNull": //空值处理   
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }

                #endregion

                rowIndex++;
            }
            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet   
                return ms;
            }
        }

        /// <summary>   
        /// DataSet导出到Excel的MemoryStream   
        /// </summary>   
        /// <param name="dsSource">源DataSet</param>
        /// <param name="hasHeader">是否设置表头</param>
        /// <param name="strHeaderText">表头文本</param> 
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        /// <param name="callBack">每个单元格的回调</param>
        /// <returns></returns>
        public static MemoryStream Export(DataSet dsSource, bool hasHeader, string strHeaderText, string errorColumn,
                                          Action<DataRow, HSSFWorkbook, HSSFSheet, HSSFCell> callBack)
        {
            var workbook = new HSSFWorkbook();

            #region 右击文件 属性信息

            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                workbook.SummaryInformation = si;
            }

            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle();
            HSSFDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");

            HSSFCellStyle errorStyle = workbook.CreateCellStyle();
            HSSFFont errorFont = workbook.CreateFont();
            errorFont.Color = HSSFColor.RED.index;
            errorStyle.SetFont(errorFont);

            for (int t = 0; t < dsSource.Tables.Count; t++)
            {
                DataTable dtSource = dsSource.Tables[t];
                HSSFSheet sheet = workbook.CreateSheet(dtSource.TableName);


                //取得列宽   
                var arrColWidth = new int[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }

                int rowIndex = 0;

                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet();
                        }

                        #region 表头及样式

                        if (hasHeader)
                        {
                            HSSFRow headerRow = sheet.CreateRow(0);
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(strHeaderText);

                            HSSFCellStyle headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                            HSSFFont font = workbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);

                            headerRow.GetCell(0).CellStyle = headStyle;

                            sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        }

                        #endregion

                        #region 列头及样式

                        {
                            int rowNum = hasHeader ? 1 : 0;
                            HSSFRow headerRow = sheet.CreateRow(rowNum);
                            HSSFCellStyle headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                            HSSFFont font = workbook.CreateFont();
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);

                            foreach (DataColumn column in dtSource.Columns)
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                                //设置列宽   
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                            rowIndex = rowNum + 1;
                        }

                        #endregion
                    }

                    #endregion

                    #region 填充内容

                    HSSFRow dataRow = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        DataColumn column = dtSource.Columns[i];
                        HSSFCell newCell = dataRow.CreateCell(column.Ordinal);
                        string drValue = row[column].ToString();
                        if (dtSource.Columns[i].ColumnName == errorColumn)
                        {
                            newCell.CellStyle = errorStyle;
                        }
                        if (callBack != null)
                        {
                            callBack(row, workbook, sheet, newCell);
                        }
                        switch (column.DataType.ToString())
                        {
                            case "System.String": //字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime": //日期类型   
                                DateTime dateV;
                                if (!String.IsNullOrEmpty(drValue))
                                {
                                    DateTime.TryParse(drValue, out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle; //格式化显示 
                                }
                                else
                                    newCell.SetCellValue("");
                                break;
                            case "System.Boolean": //布尔型   
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16": //整型   
                            case "System.Int32":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Int64":
                                long longV = 0;
                                long.TryParse(drValue, out longV);
                                newCell.SetCellValue(longV);
                                break;
                            case "System.Decimal": //浮点型   
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DBNull": //空值处理   
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }
                    }

                    #endregion

                    rowIndex++;
                }
            }
            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet   
                return ms;
            }
        }


        /// <summary>   
        /// DataTable导出到Excel的MemoryStream   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>
        /// <param name="baseModel">导出模型</param>
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        /// <returns></returns>
        public static MemoryStream Export(DataTable dtSource, ExportModel baseModel, string errorColumn)
        {
            var workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet();

            #region 右击文件 属性信息

            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                workbook.SummaryInformation = si;
            }

            #endregion

            HSSFCellStyle cellStyle = workbook.CreateCellStyle();
            if (baseModel.HasBorder)
            {
                cellStyle.BorderTop = cellStyle.BorderRight = cellStyle.BorderTop = cellStyle.BorderBottom = 1;
            }

            HSSFCellStyle dateStyle = cellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat(baseModel.DateFormat ?? "yyyy/MM/dd hh:mm:ss");

            HSSFCellStyle errorStyle = workbook.CreateCellStyle();
            HSSFFont errorFont = workbook.CreateFont();
            errorFont.Color = HSSFColor.RED.index;
            errorStyle.SetFont(errorFont);

            //取得列宽   
            var arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            int rowIndex = 0;
            int cols = dtSource.Columns.Count;
            if (baseModel.GroupTables.IsNullData() || baseModel.GroupTables.Count == 0)
            {
                baseModel.GroupTables = new Dictionary<string, DataTable>();
                baseModel.GroupTables.Add("unique", dtSource); //兼容单表导入
            }

            baseModel.ColumnCount = cols;
            baseModel.Sheet = sheet;
            baseModel.Workbook = workbook;

            #region 新建表，填充表头，填充列头，样式

            if (rowIndex == 65535 || rowIndex == 0)
            {
                if (rowIndex != 0)
                {
                    sheet = workbook.CreateSheet();
                }

                #region 表头及样式

                if (baseModel.HasHeader)
                {
                    ExportModel model = baseModel;
                    model.RowIndex = rowIndex;
                    model.RowHeight = 25;
                    model.FontSize = 20;
                    model.FontWeight = 700;
                    model.HasBorder = false;
                    model.Content = baseModel.HeaderText;
                    model.Alignment = HSSFCellStyle.ALIGN_CENTER;
                    AddMergeRow(model);
                    rowIndex++;
                }

                #endregion

                #region 创建标题行

                if (baseModel.HasBegin)
                {
                    HSSFRow headerRow = sheet.CreateRow(rowIndex);
                    ExportModel model = baseModel;
                    model.RowIndex = rowIndex;
                    model.FontSize = 10;
                    model.RowHeight = 15;
                    model.FontWeight = 700;
                    model.HasBorder = false;
                    model.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    HSSFCellStyle headStyle = CreateCellStyle(model);
                    for (int i = 0; i < baseModel.BeginText.Count; i++)
                    {
                        headerRow.CreateCell(i).SetCellValue(baseModel.BeginText[i]);
                        headerRow.GetCell(i).CellStyle = headStyle;
                        //设置列宽   
                        sheet.AddMergedRegion(new Region(model.RowIndex, i, model.RowIndex, i + 1));
                    }
                    rowIndex++;
                }

                #endregion
            }

            #endregion

            foreach (var kvp in baseModel.GroupTables)
            {
                DataTable table = kvp.Value;

                #region 列头及样式

                {
                    HSSFRow headerRow = sheet.CreateRow(rowIndex);
                    HSSFCellStyle headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                    headStyle.BorderTop = headStyle.BorderBottom = headStyle.BorderLeft = headStyle.BorderRight = 1;
                    HSSFFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 10;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);
                    foreach (DataColumn column in table.Columns)
                    {
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                        //设置列宽
                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                    }
                    rowIndex++;
                }

                #endregion

                foreach (DataRow row in table.Rows)
                {
                    #region 填充内容

                    HSSFRow dataRow = sheet.CreateRow(rowIndex);
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        DataColumn column = table.Columns[j];
                        HSSFCell newCell = dataRow.CreateCell(column.Ordinal);
                        string drValue = row[column].ToString();
                        newCell.CellStyle = cellStyle;
                        if (table.Columns[j].ColumnName == errorColumn)
                        {
                            newCell.CellStyle = errorStyle;
                        }
                        switch (column.DataType.ToString())
                        {
                            case "System.String": //字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime": //日期类型   
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);
                                newCell.CellStyle = dateStyle; //格式化显示
                                break;
                            case "System.Boolean": //布尔型   
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16": //整型   
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal": //浮点型   
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DBNull": //空值处理   
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }
                    }
                    rowIndex++;

                    #endregion
                }

                #region 填充统计行

                if (baseModel.HasStat)
                {
                    if (!baseModel.StatsTextList.IsNullData())
                    {
                        List<string> stats = baseModel.StatsTextList[kvp.Key];
                        foreach (string statText in stats)
                        {
                            ExportModel model = baseModel;
                            model.ColumnCount = baseModel.ColumnCount;
                            model.RowIndex = rowIndex;
                            model.FontSize = 10;
                            model.RowHeight = 15;
                            model.FontWeight = 200;
                            model.HasBorder = true;
                            model.Content = statText;
                            model.Alignment = HSSFCellStyle.ALIGN_CENTER;
                            AddMergeRow(model);
                            rowIndex++;
                        }
                    }
                }

                #endregion
            }

            #region 填充结尾行

            //填充结尾行
            if (baseModel.HasEnd)
            {
                foreach (string text in baseModel.EndText)
                {
                    ExportModel model = baseModel;
                    model.RowIndex = rowIndex;
                    model.FontSize = 10;
                    model.RowHeight = 15;
                    model.FontWeight = 200;
                    model.HasBorder = true;
                    model.Content = text;
                    model.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    AddMergeRow(model);
                    rowIndex++;
                }
            }

            #endregion

            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet   
                return ms;
            }
        }

        /// <summary>
        /// 添加合并行(跨所有列)
        /// </summary>
        private static void AddMergeRow(ExportModel model)
        {
            HSSFRow headerRow = model.Sheet.CreateRow(model.RowIndex);
            headerRow.HeightInPoints = model.RowHeight;
            headerRow.CreateCell(0).SetCellValue(model.Content);
            headerRow.GetCell(0).CellStyle = CreateCellStyle(model);
            var region = new CellRangeAddress(model.RowIndex, model.RowIndex, 0, model.ColumnCount - 1);
            model.Sheet.AddMergedRegion(region);

            //set enclosed border for the merged region  
            //((HSSFSheet)model.Sheet).SetEnclosedBorderOfRegion(region, CellBorderType.DOTTED, NPOI.HSSF.Util.HSSFColor.RED.index);  

            //model.Sheet.AddMergedRegion(new Region(model.RowIndex, 0, model.RowIndex, model.ColumnCount - 1));
        }

        private static HSSFCellStyle CreateCellStyle(ExportModel model)
        {
            HSSFCellStyle headStyle = model.Workbook.CreateCellStyle();
            headStyle.Alignment = model.Alignment;
            if (model.HasBorder)
            {
                headStyle.BorderTop = headStyle.BorderBottom = headStyle.BorderLeft = headStyle.BorderRight = 1;
            }
            HSSFFont font = model.Workbook.CreateFont();
            font.FontHeightInPoints = model.FontSize;
            font.Boldweight = model.FontWeight;
            headStyle.SetFont(font);
            return headStyle;
        }

        /// <summary>   
        /// 用于Web导出   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="hasHeader">是否设置表头</param>
        /// <param name="strHeaderText">表头文本</param>   
        /// <param name="strFileName">文件名</param>  
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        public static void ExportByWeb(DataTable dtSource, bool hasHeader, string strHeaderText, string strFileName,
                                       string errorColumn)
        {
            HttpContext curContext = HttpContext.Current;
            // 设置编码和附件格式   
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                                             "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));
            curContext.Response.BinaryWrite(Export(dtSource, hasHeader, strHeaderText, errorColumn).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>   
        /// 用于Web导出   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="exportModel">导出模型</param>
        /// <param name="strFileName">文件名</param>  
        /// <param name="errorColumn">错误列名(用于高亮显示错误)</param>
        public static void ExportByWeb(DataTable dtSource, ExportModel model, string strFileName, string errorColumn)
        {
            HttpContext curContext = HttpContext.Current;
            // 设置编码和附件格式   
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                                             "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));
            curContext.Response.BinaryWrite(Export(dtSource, model, errorColumn).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>   
        /// 用于Web导出   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="exportModel">导出模型</param>
        /// <param name="strFileName">文件名</param> 
        public static void ExportByWeb(DataTable dtSource, ExportModel model, string strFileName)
        {
            ExportByWeb(dtSource, model, strFileName, string.Empty);
        }

        /// <summary>   
        /// 用于Web导出   
        /// </summary>  
        /// <param name="filePath">文件存放路径</param>
        /// <param name="fileName">导出文件名</param>
        public static void ExportByWeb(string filePath, string fileName)
        {
            byte[] buffer = default(byte[]);
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Flush();
            }
            //执行删除
            Directory.Delete(filePath.Substring(0, filePath.LastIndexOf("\\")), true);
            HttpContext curContext = HttpContext.Current;
            // 设置编码和附件格式   
            curContext.Response.ContentType = "application/octet-stream";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                                             "attachment;filename=" +
                                             HttpUtility.UrlEncode(Path.GetFileName(fileName), Encoding.UTF8));

            curContext.Response.BinaryWrite(buffer);
            //curContext.Response.Clear();
            curContext.Response.End();
        }

        /// <summary>   
        /// 用于Web导出   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="hasHeader">是否设置表头</param>
        /// <param name="strHeaderText">表头文本</param>   
        /// <param name="strFileName">文件名</param>  
        public static void ExportByWeb(DataTable dtSource, bool hasHeader, string strHeaderText, string strFileName)
        {
            ExportByWeb(dtSource, hasHeader, strHeaderText, strFileName, string.Empty);
        }

        /// <summary>
        /// 用于Web导出   
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strFileName">文件名</param>
        public static void ExportByWeb(DataTable dtSource, string strFileName)
        {
            ExportByWeb(dtSource, false, string.Empty, strFileName);
        }

        /// <summary>读取excel   
        /// 默认第一行为标头   
        /// </summary>   
        /// <param name="strFileName">excel文档路径</param>   
        /// <returns></returns>   
        public static DataTable Import(string strFileName)
        {
            var dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = hssfworkbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            HSSFRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                HSSFCell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>        
        /// 导出Excel        
        /// </summary>        
        /// <param name="dgv"></param>        
        /// <param name="filePath"></param>        
        public static void ExportExcel(DataTable dtSource, string filePath)
        {
            if (ListColumnsName == null || ListColumnsName.Count == 0)
                throw (new Exception("请对ListColumnsName设置要导出的列明！"));
            HSSFWorkbook excelWorkbook = CreateExcelFile();
            InsertRow(dtSource, excelWorkbook);
            SaveExcelFile(excelWorkbook, filePath);
        }

        /// <summary>        
        /// 导出Excel        
        /// </summary>        
        /// <param name="dgv"></param>        
        /// <param name="filePath"></param>        
        public static void ExportExcel(DataTable dtSource, Stream excelStream)
        {
            if (ListColumnsName == null || ListColumnsName.Count == 0)
                throw (new Exception("请对ListColumnsName设置要导出的列明！"));
            HSSFWorkbook excelWorkbook = CreateExcelFile();
            InsertRow(dtSource, excelWorkbook);
            SaveExcelFile(excelWorkbook, excelStream);
        }

        /// <summary>        
        /// 保存Excel文件        
        /// </summary>        
        /// <param name="excelWorkBook"></param>        
        /// <param name="filePath"></param>        
        protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, string filePath)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Create);
                excelWorkBook.Write(file);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>        
        /// 保存Excel文件        
        /// </summary>        
        /// <param name="excelWorkBook"></param>        
        /// <param name="filePath"></param>        
        protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, Stream excelStream)
        {
            try
            {
                excelWorkBook.Write(excelStream);
            }
            finally
            {
            }
        }

        /// <summary>        
        /// 创建Excel文件        
        /// </summary>        
        /// <param name="filePath"></param>        
        protected static HSSFWorkbook CreateExcelFile()
        {
            var hssfworkbook = new HSSFWorkbook();
            return hssfworkbook;
        }

        /// <summary>        
        /// 创建excel表头        
        /// </summary>        
        /// <param name="dgv"></param>        
        /// <param name="excelSheet"></param>        
        protected static void CreateHeader(HSSFSheet excelSheet)
        {
            int cellIndex = 0;
            //循环导出列            
            foreach (DictionaryEntry de in ListColumnsName)
            {
                HSSFRow newRow = excelSheet.CreateRow(0);
                HSSFCell newCell = newRow.CreateCell(cellIndex);
                newCell.SetCellValue(de.Value.ToString());
                cellIndex++;
            }
        }

        /// <summary>        
        /// 插入数据行        
        /// </summary>        
        protected static void InsertRow(DataTable dtSource, HSSFWorkbook excelWorkbook)
        {
            int rowCount = 0;
            int sheetCount = 1;
            HSSFSheet newsheet = null;
            //循环数据源导出数据集            
            newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
            CreateHeader(newsheet);
            foreach (DataRow dr in dtSource.Rows)
            {
                rowCount++;
                //超出10000条数据 创建新的工作簿                
                if (rowCount == 10000)
                {
                    rowCount = 1;
                    sheetCount++;
                    newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
                    CreateHeader(newsheet);
                }
                HSSFRow newRow = newsheet.CreateRow(rowCount);
                InsertCell(dtSource, dr, newRow, newsheet, excelWorkbook);
            }
        }

        /// <summary>        
        /// 导出数据行        
        /// </summary>        
        /// <param name="dtSource"></param>        
        /// <param name="drSource"></param>        
        /// <param name="currentExcelRow"></param>        
        /// <param name="excelSheet"></param>        
        /// <param name="excelWorkBook"></param>        
        protected static void InsertCell(DataTable dtSource, DataRow drSource, HSSFRow currentExcelRow,
                                         HSSFSheet excelSheet, HSSFWorkbook excelWorkBook)
        {
            for (int cellIndex = 0; cellIndex < ListColumnsName.Count; cellIndex++)
            {
                //列名称                
                string columnsName = ListColumnsName.GetKey(cellIndex).ToString();
                HSSFCell newCell = null;
                Type rowType = drSource[columnsName].GetType();
                string drValue = drSource[columnsName].ToString().Trim();
                switch (rowType.ToString())
                {
                    case "System.String":
                        //字符串类型                        
                        drValue = drValue.Replace("&", "&");
                        drValue = drValue.Replace(">", ">");
                        drValue = drValue.Replace("<", "<");
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(drValue);
                        break;
                    case "System.DateTime":
                        //日期类型                        
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(dateV);
                        //格式化显示                        
                        HSSFCellStyle cellStyle = excelWorkBook.CreateCellStyle();
                        HSSFDataFormat format = excelWorkBook.CreateDataFormat();
                        cellStyle.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.Boolean":
                        //布尔型                        
                        bool boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(boolV);
                        break;
                    case "System.Int16":
                    //整型                    
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(intV.ToString());
                        break;
                    case "System.Decimal":
                    //浮点型                    
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(doubV);
                        break;
                    case "System.DBNull":
                        //空值处理                        
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue("");
                        break;
                    default:
                        throw (new Exception(rowType + "：类型数据无法处理!"));
                }
            }
        }

        /// <summary>
        /// 高亮显示(红色)错误列数据
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="data">数据来源DataTable</param>
        public static void HighLightErrorData(string filePath, DataTable dtSource)
        {
            if (String.IsNullOrEmpty(filePath) ||
                !File.Exists(filePath) ||
                dtSource == null || dtSource.Rows.Count == 0)
                return;
            try
            {
                HSSFWorkbook ws = default(HSSFWorkbook);
                using (var inputFile = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    ws = new HSSFWorkbook(inputFile);
                    HSSFSheet sheet = ws.GetSheet("Sheet0");
                    HSSFCellStyle style = ws.CreateCellStyle();
                    HSSFFont font = ws.CreateFont();
                    font.Color = HSSFColor.RED.index;
                    style.SetFont(font);
                    //不格式化表头
                    for (int i = 1; i <= dtSource.Rows.Count; i++)
                    {
                        //设置最后一列
                        HSSFCell cell = HSSFCellUtil.GetCell(sheet.GetRow(i), dtSource.Columns.Count - 1);
                        cell.CellStyle = style;
                    }
                }
                using (var outputFile = new FileStream(filePath, FileMode.Open))
                {
                    if (ws != null)
                    {
                        ws.Write(outputFile);
                    }
                }
            }
            catch (Exception e)
            {
                //Alert("读取Excel发生错误,原因: " + e.Message);
            }
        }

        /// <summary>
        /// 获取上传EXCEL列表
        /// </summary>
        /// <param name="txtFile">上传控件</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="batchNo">生成批次号</param>
        /// <param name="error">错误提示</param>
        /// <returns></returns>
        public static DataTable GetUploadData(HtmlInputFile txtFile, string filePath, DataTable template,
                                              out string batchNo, out string error)
        {
            DataTable data = default(DataTable);
            error = string.Empty;
            DateTime now = DateTime.Now; //~/file/UpFile
            string localDirName = string.Format(@"{0}{1}\{2}\{3}\", filePath, now.Year, now.Month, now.Day);
            batchNo = now.ToString("yyMMddHHmmss");
            //开始导入
            string localFileName = Path.Combine(localDirName, batchNo + "-OrderInfo{0}");
            localFileName = String.Format(localFileName, Path.GetExtension(txtFile.PostedFile.FileName));
            string serverDirName = string.Format(@"{0}/{1}/{2}", now.Year, now.Month, now.Day);
            string serverFileName = serverDirName + "/" + batchNo + "-OrderInfo" +
                                    Path.GetExtension(txtFile.PostedFile.FileName);

            var c = new FtpFileOperationControl();
            c.UploadFileStream = txtFile.PostedFile.InputStream;
            c.UploadFileName = serverFileName;
            c.MakeDirName = serverDirName;
            c.DownloadLocalPath = localFileName;
            c.DownLoadServerFileName = serverFileName;
            c.BeforeDownLoad += c_BeforeDownLoad;

            try
            {
                c.Do(FtpFileAction.UploadAndAccess);
            }
            catch
            {
                throw;
            }
            try
            {
                data = Excel.ExcelToDataSetFor03And07Ex(localFileName, template);
            }
            catch
            {
                //文件格式不一致，首先标准格式化文件
                data = Excel.FormatExcel2Datatable(localFileName);
            }
            //如果检测到无数据，则从本地路径中删除
            if (data == null || data.Rows.Count == 0)
            {
                error = "文件格式不对，请将数据拷贝到模板中再执行导入！";
                File.Delete(localFileName);
            }
            ////移除模板行数据(处理前2行)
            //for (var i = 0; i < 2; i++)
            //{
            //    var dr = data.Rows[i];
            //    if (dr.IsNullData()) continue;
            //    if (dr.IsTemplateRow(template, i))
            //    {
            //        data.Rows.Remove(dr);
            //    }
            //}
            return data;
        }

        private static void c_BeforeDownLoad(object sender, FtpFileOperationControl.FtpOperateEventArgs e)
        {
            if (!Directory.Exists(e.Result.ToString()))
            {
                Directory.CreateDirectory(e.Result.ToString());
            }
        }

        /// <summary>
        /// 获取上传模板
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUploadTemplate(string template, out string error)
        {
            DataTable data = default(DataTable);
            error = string.Empty;
            if (File.Exists(template))
            {
                DataSet ds = Excel.ExcelToDataSetFor03And07(template);
                data = ds != null ? ds.Tables[0] : data;
            }
            else
            {
                error = String.Format("模板不存在！请在[{0}]上传模板！", template);
            }
            return data;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void Download(string fileName)
        {
            //string serverDirName = string.Format(@"{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month,
            //DateTime.Now.Day, "ErrorFiles");
            var file = Path.GetFileName(fileName).Trim();
            if (file.IsNullData() || file.Length < 6) return;
            //string serverDirName = string.Format(@"20{0}/{1}/{2}/{3}",
            //    file.Substring(0, 2),
            //    file.Substring(2, 1) == "0" ? file.Substring(3, 1) : file.Substring(2, 2),
            //    file.Substring(4, 1) == "0" ? file.Substring(5, 1) : file.Substring(4, 2), "ErrorFiles");

            //if (!File.Exists(fileName))
            //{
            try
            {
                //Alert("该下载文件已被删除！");
                var c = new FtpFileOperationControl();
                c.DownloadLocalPath = fileName;
                c.DownLoadServerFileName = fileName;// Path.Combine(serverDirName, file);
                c.Do(FtpFileAction.Download);
                //return;
            }
            catch
            {
                throw;
            }
            //}
            //HttpContext.Current.Response.Redirect(string.Format(@"~\{0}\{1}", serverDirName,fileName));
            //HttpContext.Current.Response.Redirect(string.Format( fileName));

            var fileInfo = new FileInfo(fileName);
            HttpContext.Current.Response.ContentType = "application/ms-download";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Type", "application/octet-stream");
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.AddHeader("Content-Disposition",
                                                   "attachment;filename=" +
                                                   HttpUtility.UrlEncode(fileInfo.Name, Encoding.UTF8));
            HttpContext.Current.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            HttpContext.Current.Response.WriteFile(fileInfo.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 移除与模板不一致的空白列
        /// </summary>
        /// <param name="uploadData">上传数据</param>
        /// <param name="template">模板</param>
        /// <returns></returns>
        public static bool RemoveEmptyCell(DataTable uploadData, DataTable template)
        {
            bool valid = true;
            //移除空白列
            if (uploadData.Columns.Count > template.Columns.Count)
            {
                for (int i = uploadData.Columns.Count; i > template.Columns.Count; i--)
                {
                    uploadData.Columns.RemoveAt(i - 1);
                }
            }
            //判断上传数据列与模板列是否相等
            if (uploadData.Columns.Count != template.Columns.Count)
            {
                valid = false;
            }
            return valid;
        }

        /// <summary>
        /// 上传数据是否符合模板格式
        /// </summary>
        /// <param name="uploadData">上传数据</param>
        /// <param name="template">模板</param>
        /// <returns></returns>
        public static bool IsTemplateFormat(DataTable uploadData, DataTable template)
        {
            int col = -1;
            return IsTemplateFormat(uploadData, template, out col);
        }

        /// <summary>
        /// 上传数据是否符合模板格式
        /// </summary>
        /// <param name="uploadData">上传数据</param>
        /// <param name="template">模板</param>
        /// <param name="errorCol">错误信息</param>
        /// <returns></returns>
        public static bool IsTemplateFormat(DataTable uploadData, DataTable template, out int errorCol)
        {
            bool valid = true;
            errorCol = -1;
            if (uploadData.Columns.Count != template.Columns.Count) return false;
            for (int i = 0; i < uploadData.Columns.Count; i++)
            {
                string columnName = uploadData.Columns[i].ColumnName.Trim();
                //比较表头列名是否一致
                valid = template.Columns.Contains(columnName);
                if (!valid)
                {
                    errorCol = i;
                    break;
                }
            }
            return valid;
        }
        /// <summary>
        /// 移除末尾的空行并检测中间空行的位置
        /// </summary>
        /// <param name="uploadData">上传数据</param>
        /// <returns></returns>
        public static bool RemoveEmptyRow(DataTable uploadData, out int quantity, out int errorRow)
        {
            quantity = 0;
            errorRow = -1;
            bool valid = true;
            //比较上传数量与实际订单数量是否一致
            for (int i = uploadData.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = uploadData.Rows[i];
                if (StringUtil.IsEmptyDataRow(dr))
                {
                    //移除最后的空行
                    if (quantity == 0)
                    {
                        uploadData.Rows.RemoveAt(i);
                    }
                    else
                    {
                        errorRow = i + 2;
                        valid = false;
                        break;
                    }
                }
                else
                {
                    quantity++;
                }
            }
            return valid;
        }
    }
}
