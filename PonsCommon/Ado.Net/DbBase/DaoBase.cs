using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ado.Net.Exceptions;
using PonsUtil;

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
