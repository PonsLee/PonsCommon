using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ado.Net.Context;
using Microsoft.ApplicationBlocks.Data;
using PonsUtil;
using PonsUtil.Pager;

namespace Ado.Net.DbBase
{
    public abstract class SqlServerDao : DaoBase<SqlConnection>
    {
        public override SqlConnection GetConnection(string connstr)
        {
            SqlConnection connection = NeutralContext.Get(Constants.NcUnitofworkTransactionConnectionThread) as SqlConnection;

            if (connection != null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            else
            {
                connection = new SqlConnection(connstr);
            }

            return connection;
        }

        public virtual SqlParameter[] ToParameters(DbParameter[] parameters)
        {
            SqlParameter[] dbParameters = new SqlParameter[parameters.Length];

            DbParameter temp = null;

            SqlParameter tempOracle = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                temp = parameters[i];

                tempOracle = new SqlParameter();
                tempOracle.ParameterName = temp.ParameterName;
                tempOracle.DbType = temp.DbType;
                tempOracle.Size = temp.Size;
                tempOracle.Value = temp.Value;

                dbParameters[i] = tempOracle;
            }

            return dbParameters;
        }

        public virtual SqlParameter[] ToParameters<T>(T parameters) where T : IEnumerable
        {
            List<SqlParameter> list = new List<SqlParameter>();

            DbParameter temp = null;
            SqlParameter tempOracleServer = null;

            foreach (var item in parameters)
            {
                temp = item as DbParameter;

                tempOracleServer = new SqlParameter();
                tempOracleServer.ParameterName = temp.ParameterName;
                tempOracleServer.DbType = temp.DbType;
                tempOracleServer.Size = temp.Size;
                tempOracleServer.Value = temp.Value;

                list.Add(tempOracleServer);
            }

            return list.ToArray();
        }

        public virtual T GetId<T>(SqlConnection connection, string seqName)
        {
            var builder = new StringBuilder();

            builder.Append("select ");
            builder.Append(seqName);
            builder.Append(".Nextval from dual");

            object obj = SqlHelper.ExecuteScalar(connection, CommandType.Text, builder.ToString());

            return DataConvert.ToValue<T>(obj);
        }

        public virtual T GetId<T>(string seqName)
        {
            var builder = new StringBuilder();

            builder.Append("select ");
            builder.Append(seqName);
            builder.Append(".Nextval from dual");

            object obj = SqlHelper.ExecuteScalar(AbcExecuteConnString, CommandType.Text, builder.ToString());

            return DataConvert.ToValue<T>(obj);
        }

        public override string WrapPagingSearchSql(string innerSql, PageInfo pageInfo)
        {
            throw new NotImplementedException();
        }

        public override string WrapCountingSearchSql(string innerSql)
        {
            throw new NotImplementedException();
        }

        public override DataTable DoPaging(string oracleConnectionString, string strSql, PageInfo pageInfo, params DbParameter[] commandParameters)
        {
            throw new NotImplementedException();
        }
    }
}
