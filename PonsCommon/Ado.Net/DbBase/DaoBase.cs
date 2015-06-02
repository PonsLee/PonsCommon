using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ado.Net.Exceptions;
using Ado.Net.TmsStyle;
using Oracle.Application.Blocks.Data;
using Oracle.DataAccess.Client;
using Pons.Model;
using PonsUtil;
using PonsUtil.Pager;

namespace Ado.Net.DbBase
{
    [Serializable]
    public abstract class DaoBase<T>
    {
        public virtual string AbcExecuteConnString
        {
            get { return ConnectionStrings.AbcExecuteConnString; }
        }

        public T FMSExecuteConnection
        {
            get
            {
                return GetConnection(AbcExecuteConnString);
            }
        }



        ///// <summary>
        ///// 传入数组参数批量执行
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <param name="commandText"></param>
        ///// <param name="arrayCount"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public int ExecuteSqlArrayNonQuery(ObjectInPool<OracleConnection> connection, string commandText, int arrayCount, params OracleParameter[] commandParameters)
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return OracleHelpert.ExecuteSqlArrayNonQuery(connection.PoolObject, commandText, arrayCount, commandParameters);
        //    }
        //}

        //public int ExecuteSqlNonQuery(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters)
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return OracleHelpert.ExecuteSqlNonQuery(connection.PoolObject, commandText, commandParameters);
        //    }
        //}

        ///// <summary>
        ///// 返回分页后的结果对象列表(reader反射)
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="condition"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public PagedList<T> ExecuteSqlPaged_ByRowNumberGrammar_ByReaderReflect<T>(ObjectInPool<OracleConnection> connection, string commandText, BaseSearchModel condition, params OracleParameter[] commandParameters) where T : new()
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        PageInfo info = new PageInfo() { CurrentPageIndex = condition.PageIndex, PageSize = condition.PageSize, SortString = condition.OrderByString };
        //        IList<T> list = new List<T>().SetValueFromDB<T>(OraclePagingHelper.ExecuteSqlDataReader(connection.PoolObject, commandText, info, commandParameters));
        //        return PagedList.Create(list, info.CurrentPageIndex, info.PageSize, info.ItemCount);
        //    }
        //}

        ///// <summary>
        ///// 返回结果对象列表
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public IList<T> ExecuteSql_ByReaderReflect<T>(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters) where T : new()
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return new List<T>().SetValueFromDB<T>(OracleHelpert.ExecuteSqlReader(connection.PoolObject, commandText, commandParameters));
        //    }
        //}

        ///// <summary>
        ///// 返回结果对象列表
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public IList<T> ExecuteSql_ByDataTableReflect<T>(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters) where T : new()
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return new List<T>().SetValueFromDB<T>(OracleHelpert.ExecuteSqlDataTable(connection.PoolObject, commandText, commandParameters));
        //    }
        //}

        ///// <summary>
        ///// 返回单个结果对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public T ExecuteSqlSingle_ByDataTableReflect<T>(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters) where T : new()
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return new T().SetValueFromDB(OracleHelpert.ExecuteSqlDataTable(connection.PoolObject, commandText, commandParameters));
        //    }
        //}

        ///// <summary>
        ///// 返回单个结果对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public T ExecuteSqlSingle_ByReaderReflect<T>(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters) where T : new()
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return new T().SetValueFromDB(OracleHelpert.ExecuteSqlReader(connection.PoolObject, commandText, commandParameters));
        //    }
        //}

        ///// <summary>
        ///// 返回DataTable
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public DataTable ExecuteSqlDataTable(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters)
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return OracleHelpert.ExecuteSqlDataTable(connection.PoolObject, commandText, commandParameters);
        //    }
        //}

        ///// <summary>
        ///// 返回Scalar
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="commandText"></param>
        ///// <param name="commandParameters"></param>
        ///// <returns></returns>
        //public object ExecuteSqlScalar(ObjectInPool<OracleConnection> connection, string commandText, params OracleParameter[] commandParameters)
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        return OracleHelpert.ExecuteSqlScalar(connection.PoolObject, commandText, commandParameters);
        //    }
        //}

        public string ResetTimeOutString(string connectionString, int timeOutSecond)
        {
            if (timeOutSecond > 180)
                throw new AdoNetException("超时时间过长，建议重新设置，请低于180秒！");
            return string.Format("Connection Timeout={1};{0}", connectionString, timeOutSecond);
        }

        //Fix For Unit Of Work TransactionScrop escalate to DTC
        public abstract T GetConnection(string connstr);

        public abstract string WrapPagingSearchSql(string innerSql, PageInfo pageInfo);

        public abstract string WrapCountingSearchSql(string innerSql);

        public abstract DataTable DoPaging(string oracleConnectionString, string strSql, PageInfo pageInfo,
                                           params DbParameter[] commandParameters);
    }
}
