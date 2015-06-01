using System;
using System.Data;
using PonsUtil;

namespace Ado.Net.UnitOfWork
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkDefinition : Attribute, IUnitOfWorkDefinition
    {
        /// <summary>
        /// Default value:
        /// <table>
        /// <th><td>Property</td><td>Value</td></th>
        /// <tr><td>TransactionIsolationLevel</td><td><see cref="IsolationLevel.ReadCommitted"/></td></tr>
        /// <tr><td>TransactionTimeout</td><td>30</td></tr>
        /// <tr><td>ReadOnly</td><td><code>false</code></td></tr>
        /// <tr><td>Name</td><td><code>null</code></td></tr>
        /// </table>
        /// </summary>
        public static IUnitOfWorkDefinition DefaultDefinition
        {
            get { return new DefaultAttribute(); }
        }

        #region IUnitOfWorkDefinition Members

        public System.Transactions.IsolationLevel TransactionIsolationLevel { get; set; }

        public int TransactionTimeout { get; set; }

        public bool ReadOnly { get; set; }

        public string Name { get; set; }

        public bool Exclude { get; set; }

        public DataBaseType DBType { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}:({1})",
                                 base.ToString(),
                                 new JsonObjectFormatter().Format(null, this, null));
        }

        #region Nested type: DefaultAttribute
        public class DefaultAttribute : UnitOfWorkDefinition
        {
            public DefaultAttribute()
            {
                TransactionIsolationLevel = global::System.Transactions.IsolationLevel.ReadCommitted;
                TransactionTimeout = 60;
                DBType = DataBaseType.SqlServer;
                ReadOnly = false;
                Name = null;
                Exclude = false;
            }
        }
        #endregion
        #region Nested type: ReadOnlyDbAttribute

        /// <summary>
        /// 
        /// </summary>
        public class CustomDbTypeAttribute : DefaultAttribute
        {
            public CustomDbTypeAttribute(DataBaseType dataBaseType)
            {
                DBType = dataBaseType;
            }
        }

        #endregion
    }
}
