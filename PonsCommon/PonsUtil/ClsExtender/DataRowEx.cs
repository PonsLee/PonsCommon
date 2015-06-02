using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.ClsExtender
{
    public static class DataRowEx
    {
        /// <summary>
        /// 判断DataRow整行的所有列是否全部为空
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsRowEmpty(this DataRow row)
        {
            foreach (var item in row.ItemArray)
            {
                if (!String.IsNullOrEmpty(item.ToString().Trim()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Trim整个DataRow的字符串
        /// </summary>
        /// <param name="row"></param>
        public static void RowTrim(this DataRow row)
        {
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                row[i] = row[i].ToString().Trim();
            }
        }

        public static T GetValue<T>(this DataRow row, string name)
        {
            //if (row == null)
            //{
            //    return default(T);
            //}
            //if (!row.Table.Columns.Contains(name))
            //{
            //    return default(T);
            //}
            object o = row[name];
            if (o == null || o == DBNull.Value)
            {
                return default(T);
            }
            Type type = typeof(T);
            if (type.Name.ToLower().Contains("nullable"))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, Convert.ToInt32(o).ToString());

            }
            return (T)Convert.ChangeType(o, type);
        }

    }
}
