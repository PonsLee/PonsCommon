using System.Transactions;

namespace Ado.Net.UnitOfWork
{
    /// <summary>
    /// Much based on TransactionDefinition of Spring.NET. But add <see cref="DatabaseSource"/>
    /// and <see cref="Exclude"/> support.
    /// </summary>
    public interface IUnitOfWorkDefinition
    {
        /// <summary>
        /// 事务隔离级别
        /// </summary>
        IsolationLevel TransactionIsolationLevel { get; }

        /// <summary>
        /// 事务超时时间
        /// </summary>
        int TransactionTimeout { get; }

        /// <summary>
        /// 告知Nhibernate此session为ReadOnly
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// 事务名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 该事务及内嵌声明式事务是否被排除，此属性对于直接Using不起作用
        /// </summary>
        bool Exclude { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DataBaseType DBType { get; }
    }
}
