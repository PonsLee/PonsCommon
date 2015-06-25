using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;

namespace PonsUtil.Converter
{
    public static class PonsConverter
    {
        public static T ConvertModel<T, M>(M originalModel) where T : new()
        {
            if (originalModel == null)
            {
                return default(T);
            }
            T t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] piTs = t.GetType().GetProperties();
            Type typeM = originalModel.GetType();
            PropertyInfo piM = null;
            foreach (PropertyInfo piT in piTs)
            {
                if (!piT.CanWrite)
                {
                    continue;
                }
                piM = typeM.GetProperty(piT.Name, piT.PropertyType);
                if (piM == null)
                {
                    continue;
                }
                piT.SetValue(t, piM.GetValue(originalModel, null), null);
            }
            return t;
        }

        public static IList<T> ConvertModelList<T, M>(IList<M> originalModelList) where T : new()
        {
            if (originalModelList == null || originalModelList.Count == 0)
            {
                return null;
            }
            IList<T> lt = new List<T>();
            foreach (M m in originalModelList)
            {
                lt.Add(ConvertModel<T, M>(m));
            }
            return lt;
        }

        public static IList<T> ConvertToFatherListModel<T, M>(this IList<M> sonList) where M : T
        {
            IList<T> lt = new List<T>();
            foreach (M m in sonList)
            {
                lt.Add((T)m);
            }
            return lt;
        }

        public static T[] ConvertArray<T>(this string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                return null;
            }
            Type type = typeof(T);
            MethodInfo mi = type.GetMethod("Parse", new Type[] { typeof(string) });
            if (mi == null)
            {
                return null;
            }
            T[] arrT = new T[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                arrT[i] = (T)mi.Invoke(null, new object[] { strs[i] });
            }
            return arrT;
        }

        public static DataTable Array2ToDataTable(String[,] array, bool firstIsCloumn)
        {
            if (array == null) throw new ArgumentNullException();
            DataTable dt = new DataTable();
            //列数
            int cc = array.GetLength(1);
            if (firstIsCloumn) if (cc == 0) throw new Exception("数据中至少有一行数据！");

            for (int i = 0; i < cc; i++)
            {
                var column = new DataColumn(firstIsCloumn ? array[0, i] : i.ToString());
                dt.Columns.Add(column);
            }

            for (int i = firstIsCloumn ? 1 : 0; i < array.GetLength(0); i++)
            {
                var row = dt.NewRow();
                for (int j = 0; j < cc; j++)
                {
                    row[j] = array[i, j];
                }
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
