using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ado.Net.UnitOfWork
{
    public class AdoNetUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region IUnitOfWorkFactory Members

        public IUnitOfWork GetInstance(IUnitOfWorkDefinition definition)
        {
            return new AdoNetUnitOfWork(new DefaultTransactionScopeFactory(definition));
        }

        public IUnitOfWork GetInstance(IUnitOfWorkDefinition definition, string connectionString)
        {
            return new AdoNetUnitOfWork(new DefaultTransactionScopeFactory(definition), connectionString);
        }

        #endregion
    }
}
