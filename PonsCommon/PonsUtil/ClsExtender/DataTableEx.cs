using System.Data;

namespace PonsUtil.ClsExtender
{
    public static class DataTableEx
    {
        /// <summary>
        /// 根据传入列名顺序，改变列名
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="arrColumns"></param>
        public static void ChangeColumnNames(this DataTable dtSource, string[] arrColumns)
        {
            if (arrColumns != null)
            {
                for (int i = 0; i < arrColumns.Length; i++)
                {
                    dtSource.Columns[i].ColumnName = arrColumns[i];
                }
            }
        }

        /// <summary>
        /// 格式化Trim列名
        /// </summary>
        /// <param name="dtSource"></param>
        public static void ColumNameTrim(this DataTable dtSource)
        {
            for (int i = 0; i < dtSource.Columns.Count; i++)
            {
                dtSource.Columns[i].ColumnName = dtSource.Columns[i].ColumnName.Trim();
            }
        }

        /// <summary>
        /// 移除DataTable最后的空行
        /// </summary>
        /// <param name="dtSource"></param>
        public static void RemoveLastEmptyRow(this DataTable dtSource)
        {
            for (int i = dtSource.Rows.Count - 1; i >= 0; i--)
            {
                if (!dtSource.Rows[i].IsRowEmpty())
                {
                    break;
                }
                dtSource.Rows.RemoveAt(i);
            }
        }

    }
}
