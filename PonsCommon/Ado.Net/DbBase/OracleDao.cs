using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using Ado.Net.Context;
using Oracle.Application.Blocks.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using PonsUtil;

namespace Ado.Net.DbBase
{
    public abstract class OracleDao : DaoBase<OracleConnection>
    {
        public override OracleConnection GetConnection(string connstr)
        {
            OracleConnection connection = NeutralContext.Get(Constants.NcUnitofworkTransactionConnectionThread) as OracleConnection;

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = new OracleConnection(connstr);
            }

            return connection;
        }

        public virtual OracleParameter[] ToParameters(DbParameter[] parameters)
        {
            OracleParameter[] dbParameters = new OracleParameter[parameters.Length];

            DbParameter temp = null;
            OracleParameter tempOracle = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                temp = parameters[i];

                tempOracle = new OracleParameter();
                tempOracle.ParameterName = temp.ParameterName;
                tempOracle.DbType = temp.DbType;
                tempOracle.Size = temp.Size;
                tempOracle.Value = temp.Value;

                dbParameters[i] = tempOracle;
            }

            return dbParameters;
        }

        public virtual OracleParameter[] ToParameters<T>(T parameters) where T : IEnumerable
        {
            List<OracleParameter> list = new List<OracleParameter>();

            DbParameter temp = null;
            OracleParameter tempOracleServer = null;

            foreach (var item in parameters)
            {
                temp = item as DbParameter;

                tempOracleServer = new OracleParameter();
                tempOracleServer.ParameterName = temp.ParameterName;
                tempOracleServer.DbType = temp.DbType;
                tempOracleServer.Size = temp.Size;
                tempOracleServer.Value = temp.Value;

                list.Add(tempOracleServer);
            }

            return list.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="seqName"></param>
        /// <returns></returns>
        public virtual T GetId<T>(OracleConnection connection, string seqName)
        {
            var builder = new StringBuilder();

            builder.Append("select ");
            builder.Append(seqName);
            builder.Append(".Nextval from dual");

            object obj = OracleHelper.ExecuteScalar(connection, CommandType.Text, builder.ToString());

            return DataConvert.ToValue<T>(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seqName"></param>
        /// <returns></returns>
        public virtual T GetId<T>(string seqName)
        {
            var builder = new StringBuilder();

            builder.Append("select ");
            builder.Append(seqName);
            builder.Append(".Nextval from dual");

            object obj = OracleHelper.ExecuteScalar(AbcExecuteConnString, CommandType.Text, builder.ToString());

            return DataConvert.ToValue<T>(obj);
        }

        public virtual List<T> GetId<T>(string seqName, int count)
        {
            var builder = new StringBuilder();
            builder.Append("select ");
            builder.Append(seqName);
            builder.Append(".Nextval from ");
            builder.Append(@"(select ot.ORDERFORTHIRDPARTYID
                               from cloud_dps.orderforthirdparty ot
                              where rownum <= :num
                              order by ot.ORDERFORTHIRDPARTYID desc) ");
            OracleParameter[] parameters = { new OracleParameter(":num", OracleDbType.Int32) { Value = count }, };
            DataTable dt = OracleHelper.ExecuteDataset(AbcExecuteConnString, CommandType.Text, builder.ToString(), parameters).Tables[0];
            List<T> list = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                list = new List<T>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(DataConvert.ToValue<T>(dr["NEXTVAL"]));
                }
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerSql"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public override string WrapPagingSearchSql(string innerSql, PageInfo pageInfo)
        {
            return
                string.Format(
                    @"SELECT *
                            FROM
                            (
                                SELECT InnerPagingTable.*,ROW_NUMBER() OVER(ORDER BY {0}) AS SortRowID
                                FROM (
                                   {1}
                                ) InnerPagingTable
                            ) 
                            WHERE SortRowID BETWEEN {2} AND {3}"
                    , String.IsNullOrEmpty(pageInfo.SortString) ? pageInfo.KeyColumn : pageInfo.SortString
                    , innerSql
                    , pageInfo.PageSize * (pageInfo.CurrentPageIndex - 1) + 1
                    , pageInfo.PageSize * pageInfo.CurrentPageIndex
                    );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerSql"></param>
        /// <returns></returns>
        public override string WrapCountingSearchSql(string innerSql)
        {
            return string.Format("SELECT COUNT(*) as AllCount FROM ({0}) t", innerSql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oracleConnectionString"></param>
        /// <param name="strSql"></param>
        /// <param name="pageInfo"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public override DataTable DoPaging(string oracleConnectionString, string strSql, PageInfo pageInfo,
                                       params DbParameter[] commandParameters)
        {
            return GetPagingData(oracleConnectionString, strSql, pageInfo,
                            commandParameters.ToList().ConvertAll(p => (OracleParameter)p).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oracleConnectionString"></param>
        /// <param name="strSql"></param>
        /// <param name="pageInfo"></param>
        /// <param name="oracleCommandParameters"></param>
        /// <returns></returns>
        public DataTable GetPagingData(string oracleConnectionString, string strSql, PageInfo pageInfo, params OracleParameter[] oracleCommandParameters)
        {
            var oracleConnection = new OracleConnection(oracleConnectionString);
            pageInfo.PageSize = pageInfo.PageSize > 0 ? pageInfo.PageSize : pageInfo.DefaultPageSize;
            pageInfo.CurrentPageIndex = pageInfo.CurrentPageIndex > 0 ? pageInfo.CurrentPageIndex : 1;
            var strSqlCount = WrapCountingSearchSql(strSql);
            var rowCount = Convert.ToInt32(OracleHelper.ExecuteScalar(oracleConnection, CommandType.Text, strSqlCount, oracleCommandParameters));
            pageInfo.SetItemCount(rowCount);
            var strSqlPage = WrapPagingSearchSql(strSql, pageInfo);
            oracleConnection = new OracleConnection(oracleConnectionString);
            return OracleHelper.ExecuteDataset(oracleConnection, CommandType.Text, strSqlPage, DepthCopy(oracleCommandParameters)).Tables[0];
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

        /// <summary>
        /// The need for this method is highly annoying.
        /// When Oracle sets its output parameters, the OracleParameter.Value property
        /// is set to an internal Oracle type, not its equivelant System type.
        /// For example, strings are returned as OracleString, DBNull is returned
        /// as OracleNull, blobs are returned as OracleBinary, etc...
        /// So these Oracle types need unboxed back to their normal system types.
        /// </summary>
        /// <param name="oracleType">Oracle type to unbox.</param>
        /// <returns></returns>
        protected static object UnBoxOracleType(object oracleType)
        {
            if (oracleType == null)
                return null;

            Type T = oracleType.GetType();
            if (T == typeof(OracleString))
            {
                if (((OracleString)oracleType).IsNull)
                    return null;
                return ((OracleString)oracleType).Value;
            }
            else if (T == typeof(OracleDecimal))
            {
                if (((OracleDecimal)oracleType).IsNull)
                    return null;
                return ((OracleDecimal)oracleType).Value;
            }
            else if (T == typeof(OracleBinary))
            {
                if (((OracleBinary)oracleType).IsNull)
                    return null;
                return ((OracleBinary)oracleType).Value;
            }
            else if (T == typeof(OracleBlob))
            {
                if (((OracleBlob)oracleType).IsNull)
                    return null;
                return ((OracleBlob)oracleType).Value;
            }
            else if (T == typeof(OracleDate))
            {
                if (((OracleDate)oracleType).IsNull)
                    return null;
                return ((OracleDate)oracleType).Value;
            }
            else if (T == typeof(OracleTimeStamp))
            {
                if (((OracleTimeStamp)oracleType).IsNull)
                    return null;
                return ((OracleTimeStamp)oracleType).Value;
            }
            else // not sure how to handle these.
                return oracleType;
        }
    }
}
