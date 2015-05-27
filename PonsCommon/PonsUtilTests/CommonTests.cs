using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PonsUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PonsUtil.Tests
{
    [TestClass()]
    public class CommonTests
    {
        [TestMethod()]
        public void GetOracleInParameterWhereSqlTest()
        {
            string sql = PonsUtil.Common.GetOracleInParameterWhereSql("waybillno", "123,4242,13131,24242,4242", true, false);

        }
    }
}
