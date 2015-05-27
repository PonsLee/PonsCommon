using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;
using System.Web;

namespace PonsUtil
{
    /// <summary>
    /// 公用方法
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 判断DataSet有无数据
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public static bool DataSetIsEmpty(DataSet ds)
        {
            return (ds == null) || (ds.Tables.Count < 1) || DataTableIsEmpty(ds.Tables[0]);
        }

        /// <summary>
        /// 判断DataTable有无数据
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool DataTableIsEmpty(DataTable dt)
        {
            return (dt == null) || (dt.Rows.Count < 1);
        }

        /// <summary>
        /// 字段值数组转换为字符串
        /// </summary>
        /// <param name="arr">字段值数组</param>
        /// <returns>以逗号拼接的查询语句</returns>
        public static string ValueArrayToQueryString(ArrayList arr)
        {
            if (arr == null || arr.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string obj in arr)
            {
                sb.AppendFormat("'{0}'{1}", obj, (i < arr.Count - 1) ? "," : "");
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字段值数组转换为字符串
        /// </summary>
        /// <param name="arr">字段值数组</param>
        /// <param name="whatever">兼容旧API，该值不做处理</param>
        /// <returns>以逗号拼接的查询语句</returns>
        public static string ValueArrayToQueryString(IList<string> arr, bool whatever)
        {
            return ValueArrayToQueryString(arr);
        }

        /// <summary>
        /// 字段值数组转换为字符串
        /// </summary>
        /// <param name="arr">字段值数组</param>
        /// <returns>以逗号拼接的查询语句</returns>
        public static string ValueArrayToQueryString(IList<string> arr)
        {
            if (arr == null || arr.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string obj in arr)
            {
                sb.AppendFormat("'{0}'{1}", obj, (i < arr.Count - 1) ? "," : "");
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// DataTable第一行插入默认名称和值(如"请选择...","0")
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="headerValue"></param>
        /// <param name="headerText"></param>
        public static void DataTableAddHeader(DataTable dt, string headerValue, string headerText)
        {
            if (dt != null && dt.Columns.Count > 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = headerValue;
                dr[1] = headerText;
                dt.Rows.InsertAt(dr, 0);
            }
            else
            {
                throw new Exception("DataTable must be 2 columns");
            }
        }
        /// <summary>
        /// DataTable添加最后一行记录
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        public static void DataTableAddRow(DataTable dt, string value, string text)
        {
            if (dt != null && dt.Columns.Count > 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = value;
                dr[1] = text;
                dt.Rows.Add(dr);
            }
            else
            {
                throw new Exception("DataTable Add rows error.");
            }

        }
        /// <summary>
        /// 合并DataSet中的多个DataTable的数据到第0个DataTable，要求DataTable的结构完全相同
        /// </summary>
        /// <param name="ds"></param>
        public static DataTable MegerDataTableInDataSet(DataSet ds)
        {
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                foreach (DataRow dr in ds.Tables[i].Rows)
                {
                    ds.Tables[0].ImportRow(dr);
                }
            }
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                ds.Tables.RemoveAt(i);
            }
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        public static string DataTableToSql(DataTable table, string insertSql)
        {
            var sb = new StringBuilder();
            foreach (DataRow row in table.Rows)
            {
                var values = new StringBuilder();
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (j > 0) values.Append(", ");
                    if (row.IsNull(j) || table.Columns[j].DataType.Name == "Byte[]")
                        values.Append("NULL");
                    else if (table.Columns[j].DataType == typeof(int) ||
                             table.Columns[j].DataType == typeof(decimal) ||
                             table.Columns[j].DataType == typeof(long) ||
                             table.Columns[j].DataType == typeof(double) ||
                             table.Columns[j].DataType == typeof(float) ||
                             table.Columns[j].DataType == typeof(byte))
                        values.Append(row[j].ToString());
                    else
                        values.AppendFormat("'{0}'",
                            row[j].ToString().Replace("\\", "\\\\").Replace("'", "''"));
                }
                sb.AppendFormat(insertSql, row[0], values);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字段值数组转换为字符串
        /// </summary>
        /// <param name="arr">字段值数组 int</param>
        /// <returns>以逗号拼接的查询语句</returns>
        public static string ValueIntArrayToQueryString(IList<int> arr)
        {
            if (arr == null || arr.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (int obj in arr)
            {
                sb.AppendFormat("{0}{1}", obj, (i < arr.Count - 1) ? "," : "");
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把datatable转换为modellist
        /// 调用此函数请保证一下几点：
        /// 1.model的内不包含子model。
        /// 2.model有无参构造函数
        /// 3.model的属性名和datatable的列名一致（不区分大小写）
        /// </summary>
        /// <typeparam name="T">model的类型</typeparam>
        /// <param name="dt">datatable</param>
        /// <returns></returns>
        public static List<T> ConvertDataTableToModelList<T>(DataTable dt) where T : new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> lstReturn = new List<T>();
            DataColumnCollection dcc = dt.Columns;
            foreach (DataRow dr in dt.Rows)
            {
                lstReturn.Add(ConvertDataRowToModel<T>(dr, dcc));
            }
            return lstReturn;
        }

        /// <summary>
        /// 把datarow转换为model
        /// 调用此函数请保证一下几点：
        /// 1.model的内不包含子model。
        /// 2.model有无参构造函数
        /// 3.model的属性名和datatable的列名一致（不区分大小写）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="dcc"></param>
        /// <returns></returns>
        public static T ConvertDataRowToModel<T>(DataRow dr, DataColumnCollection dcc) where T : new()
        {
            if (dr == null)
            {
                return default(T);
            }
            T t = new T();
            PropertyInfo[] pis = t.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                foreach (DataColumn dc in dcc)
                {
                    if (dc.ColumnName.ToLower() == pi.Name.ToLower())
                    {

                        if (dr[dc] != null && dr[dc] != DBNull.Value)
                        {
                            Type type = pi.PropertyType;
                            if (type.Name.ToLower().Contains("nullable"))
                            {
                                type = Nullable.GetUnderlyingType(type);
                            }
                            pi.SetValue(t, Convert.ChangeType(dr[dc], type), null);
                        }
                        break;
                    }
                }
            }
            return t;
        }

        public static bool IsEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        public static bool JudgeDateAreaByDay(DateTime dateS, DateTime dateE)
        {
            TimeSpan day = dateE - dateS;
            return day.TotalDays < 0 ? true : false;
        }
        public static string GetCurrentPage()
        {
            return HttpContext.Current.Request.Url.ToString().Split(new string[] { ".aspx" }, StringSplitOptions.RemoveEmptyEntries).GetValue(0).ToString().Substring(HttpContext.Current.Request.Url.ToString().Split(new string[] { ".aspx" }, StringSplitOptions.RemoveEmptyEntries).GetValue(0).ToString().LastIndexOf('/') + 1)
            + ".aspx";
        }

        /// <summary>
        /// oracle in 绑定变量
        /// </summary>
        /// <param name="columnName">查询字段名称</param>
        /// <param name="parameterName">变量名称 不需要：</param>
        /// <param name="isNotIn">true为not in，false为in</param>
        /// <param name="isOr">true为or，false为and</param>
        /// <returns></returns>
        public static string GetOracleInParameterWhereSql(string columnName, string parameterName, bool isNotIn, bool isOr)
        {
            var inOrNotinStr = isNotIn ? " NOT IN " : " IN ";
            var orOrAndStr = isOr ? " OR " : " AND ";
            //string inWhereSql = " {0} {1} {2} (select regexp_substr(:{3},'[^,]+',1,level) as value_id from dual connect by level<=length(trim(translate(:{3},translate(:{3},',',' '),' ')))+1) ";
            string inWhereSql = @" {0} {1} {2} (
                SELECT SUBSTR(inlist,
                              INSTR(inlist, ',', 1, LEVEL) + 1,
                              INSTR(inlist, ',', 1, LEVEL + 1) -
                              INSTR(inlist, ',', 1, LEVEL) - 1) AS value_str
                  FROM (SELECT ',' ||
                               :{3} || ',' AS inlist
                          FROM DUAL)
                CONNECT BY LEVEL <= LENGTH(:{3}) -
                           LENGTH(REPLACE(','||:{3},
                                                   ',',
                                                   '')) + 1)
                ";
            return string.Format(inWhereSql, orOrAndStr, columnName, inOrNotinStr, parameterName);
        }
    }
}
