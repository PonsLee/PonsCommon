using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;

namespace PonsUtil.Export
{
    public class ExportModel
    {
        public int ColumnCount { get; set; }
        public int RowHeight { get; set; }
        public int RowIndex { get; set; }
        public string Content { get; set; }
        public bool HasHeader { get; set; }
        public string HeaderText { get; set; }
        public bool HasBegin { get; set; }
        public List<string> BeginText { get; set; }
        public bool HasStat { get; set; }
        public Dictionary<string, List<string>> StatsTextList { get; set; }
        public bool HasEnd { get; set; }
        public List<string> EndText { get; set; }
        public Dictionary<string, DataTable> GroupTables { get; set; }
        public bool HasBorder { get; set; }
        public HSSFWorkbook Workbook { get; set; }
        public HSSFSheet Sheet { get; set; }
        public short FontSize { get; set; }
        public short FontWeight { get; set; }
        public short Alignment { get; set; }
        public string DateFormat { get; set; }
    }
}
