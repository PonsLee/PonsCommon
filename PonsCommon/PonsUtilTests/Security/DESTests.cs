using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PonsUtil.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PonsUtil.Security.Tests
{
    [TestClass()]
    public class DESTests
    {
        [TestMethod()]
        public void Encrypt3DESTest()
        {
            var a = ".<dVc";
            var str = DES.Encrypt3DES(a);
            var test = DES.Decrypt3DES(str);
            Assert.AreEqual(a, test);
        }
    }
}
