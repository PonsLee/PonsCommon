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
    public class MessageCollectorTests
    {
        [TestMethod()]
        public void CollectTest()
        {
            var type = GetType();
            MessageCollector.Instance.Collect(type, new Exception("123"), false);
        }
    }
}
