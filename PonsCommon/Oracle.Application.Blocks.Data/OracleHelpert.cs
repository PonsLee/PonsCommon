using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using PonsUtil.Pager;

namespace Oracle.Application.Blocks.Data
{
    public class OracleHelpert
    {
        
        private OracleHelpert()
        {
        }

        private static void PrepareCommand(OracleCommand command, OracleConnection connection, CommandType commandType, string commandText, OracleParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (connection.State != ConnectionState.Open)
            {
                //if inner open then must close this conn
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            command.Connection = connection;
            command.BindByName = true;
            command.CommandType = commandType;
            command.CommandText = commandText;
            if (commandParameters != null)
            {
                //DetachParameter
                command.Parameters.Clear();
                AttachParameters(command, commandParameters);
            }
            return;
        }

        private static void AttachParameters(DbCommand command, DbParameter[] commandParameters)
        {
            foreach (DbParameter p in commandParameters)
            {
                if ((p.Direction == ParameterDirection.InputOutput ||
                          p.Direction == ParameterDirection.Input) &&
                          (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// 执行参数化PL-SQL语句
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteSqlNonQuery(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteSqlNonQuery(new OracleConnection(connectionString), commandText, commandParameters);
        }

        /// <summary>
        /// 执行参数化PL-SQL语句
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteSqlNonQuery(OracleConnection conn, string commandText, params OracleParameter[] commandParameters)
        {
            if (conn == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            //
            PrepareCommand(
                cmd,
                conn,
                CommandType.Text,
                commandText,
                commandParameters,
                out mustCloseConnection);
            try
            {
                int retval = cmd.ExecuteNonQuery();
                return retval;
            }
            catch (OracleException ex)
            {
                throw ex;
            }
            finally
            {
                if (mustCloseConnection && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 传入数组参数批量执行
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="arrayCount"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteSqlArrayNonQuery(OracleConnection conn, string commandText, int arrayCount, params OracleParameter[] commandParameters)
        {
            if (conn == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            if (arrayCount <= 0) throw new Exception("参数数组的大小必须设置");
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            cmd.ArrayBindCount = arrayCount;
            PrepareCommand(
                cmd,
                conn,
                CommandType.Text,
                commandText,
                commandParameters,
                out mustCloseConnection);
            try
            {
                int retval = cmd.ExecuteNonQuery();
                return retval;
            }
            catch (OracleException ex)
            {
                throw ex;
            }
            finally
            {
                if (mustCloseConnection && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 执行PL-SQL参数化DataReader
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteSqlReader(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteSqlReader(new OracleConnection(connectionString), commandText, commandParameters);
        }

        /// <summary>
        /// 执行PL-SQL参数化DataReader
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteSqlReader(OracleConnection conn, string commandText, params OracleParameter[] commandParameters)
        {
            if (conn == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            bool mustCloseConnection = false;
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd
                    , conn
                    , CommandType.Text
                    , commandText
                    , commandParameters
                    , out mustCloseConnection);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (OracleException ex)
            {
                //由外部DbDataReader的Close方法关闭数据库连接,出现异常然后强制关闭
                if (mustCloseConnection && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                throw ex;
            }
        }

        /// <summary>
        /// 执行PL-SQL参数化DataSet
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteSqlDataset(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteSqlDataset(new OracleConnection(connectionString), commandText, commandParameters);
        }

        /// <summary>
        /// 执行PL-SQL参数化DataSet
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteSqlDataset(OracleConnection conn, string commandText, params OracleParameter[] commandParameters)
        {
            if (conn == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            bool mustCloseConnection = false;
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd
              , conn
              , CommandType.Text
              , commandText
              , commandParameters,
              out mustCloseConnection);
            using (DbDataAdapter adapter = new OracleDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (mustCloseConnection && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行PL-SQL参数化DataTable
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteSqlDataTable(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteSqlDataTable(new OracleConnection(connectionString), commandText, commandParameters);
        }


        /// <summary>
        /// 执行PL-SQL参数化DataTable
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteSqlDataTable(OracleConnection conn, string commandText, params OracleParameter[] commandParameters)
        {
            DataSet ds = ExecuteSqlDataset(conn, commandText, commandParameters);
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 执行PL-SQL参数化ExecuteScalar
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteSqlScalar(string connectionString, string commandText,params OracleParameter[] commandParameters)
        {
            return ExecuteSqlScalar(new OracleConnection(connectionString), commandText, commandParameters);
        }

        /// <summary>
        /// 执行PL-SQL参数化ExecuteScalar
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteSqlScalar(OracleConnection conn, string commandText, params OracleParameter[] commandParameters)
        {
            if (conn == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            OracleCommand cmd = new OracleCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd
                , conn
                , CommandType.Text
                , commandText
                , commandParameters,
                out mustCloseConnection);
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (OracleException ex)
            {
                throw ex;
            }
            finally
            {
                if (mustCloseConnection && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

    }

    public sealed class OraclePagingHelper
    {
        /// <summary>
        /// 默认每页十条数据
        /// </summary>
        private static readonly int DefaultPageSize = 10;
        /// <summary>
        /// 默认第一页
        /// </summary>
        private static readonly int DefaultPageIndex = 1;


        /// <summary>
        /// Do the get row count work
        /// </summary>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandText">The original T-SQL command</param>
        /// <param name="pageInfo">A PageInfo Model containing how to page the resultset</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        private static void DoGetRowCount(OracleConnection connection, string commandText, PageInfo pageInfo, params OracleParameter[] commandParameters)
        {
            pageInfo.PageSize = pageInfo.PageSize > 0 ? pageInfo.PageSize : DefaultPageSize;
            pageInfo.CurrentPageIndex = pageInfo.CurrentPageIndex > 0 ? pageInfo.CurrentPageIndex : DefaultPageIndex;
            int rowCount = GetRowCount(connection, commandText, commandParameters);
            pageInfo.SetItemCount(rowCount);
        }

        /// <summary>
        /// Oracle Row Numberp
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        private static string GetPagedPLSqlByRowNumberGrammar(string commandText, PageInfo pageInfo)
        {
            if (String.IsNullOrWhiteSpace(commandText)) throw new ArgumentNullException("commandText is empty");
            if (pageInfo == null) throw new ArgumentNullException("pageInfo is empty");
            return string.Format(@"
SELECT *
FROM
(
    SELECT InnerPagingTempTable.*,ROW_NUMBER() OVER(ORDER BY {0}) AS SortRowID
    FROM (
       {1}
    ) InnerPagingTempTable
) 
WHERE SortRowID BETWEEN {2} AND {3}
"
                , String.IsNullOrEmpty(pageInfo.SortString) ? pageInfo.KeyColumn : pageInfo.SortString
                , commandText
                , pageInfo.PageSize * (pageInfo.CurrentPageIndex - 1) + 1
                , pageInfo.PageSize * pageInfo.CurrentPageIndex
 );
        }

        /// <summary>
        /// 执行分页带参数PL-SQL 返回DataReader
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="pageInfo"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteSqlDataReader(OracleConnection conn, string commandText, PageInfo pageInfo, params OracleParameter[] commandParameters)
        {
            if (conn == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            if (pageInfo == null) throw new ArgumentNullException("pageInfo");
            DoGetRowCount(conn, commandText, pageInfo, commandParameters);
            return OracleHelpert.ExecuteSqlReader(conn, GetPagedPLSqlByRowNumberGrammar(commandText, pageInfo), DepthCopy(commandParameters));
        }

        /// <summary>
        /// 执行分页带参数PL-SQL 返回DataReader
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="commandText"></param>
        /// <param name="pageInfo"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteSqlDataReader(string connStr, string commandText, PageInfo pageInfo, params OracleParameter[] commandParameters)
        {
            return ExecuteSqlDataReader(new OracleConnection(connStr), commandText, pageInfo, commandParameters);
        }

        /// <summary>
        /// 执行分页不带参数PL-SQL 返回DataReader
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteSqlDataReader(OracleConnection conn, string commandText, PageInfo pageInfo)
        {
            return ExecuteSqlDataReader(conn, commandText, pageInfo, null);
        }

        /// <summary>
        /// Get the total row count
        /// </summary>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandText">The original T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>The row count</returns>
        private static int GetRowCount(OracleConnection connection, string commandText, params OracleParameter[] commandParameters)
        {
            string strSql = string.Format("SELECT COUNT(*) as CC FROM ({0}) t", commandText);
            return Convert.ToInt32(OracleHelpert.ExecuteSqlScalar(connection, strSql, commandParameters));
        }

        /// <summary>
        /// Depth copy the array of SqlParameter
        /// </summary>
        /// <param name="arguments">An array of SqlParamters used to be copy</param>
        /// <returns>A copied array of SqlParamters</returns>
        private static OracleParameter[] DepthCopy(OracleParameter[] arguments)
        {
            if (arguments != null)
            {
                List<OracleParameter> copyArgs = new List<OracleParameter>();
                foreach (OracleParameter item in arguments)
                {
                    copyArgs.Add(new OracleParameter()
                    {
                        ParameterName = item.ParameterName,
                        DbType = item.DbType,
                        OracleDbType = item.OracleDbType,
                        Direction = item.Direction,
                        Size = item.Size,
                        Value = item.Value,
                        OracleDbTypeEx = item.OracleDbTypeEx
                    });
                }
                return copyArgs.ToArray();
            }
            return null;
        }
    }
}
