using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ado.Net.UnitOfWork
{
    /// <summary>
    /// Create a unit of work based on a transaction definition.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetInstance(IUnitOfWorkDefinition definition);
    }
}
