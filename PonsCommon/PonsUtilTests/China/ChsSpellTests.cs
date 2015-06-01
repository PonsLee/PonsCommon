using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PonsUtil.China;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PonsUtil.China.Tests
{
    [TestClass()]
    public class ChsSpellTests
    {
        [TestMethod()]
        public void GetChsSpellTest()
        {
            var name = "开封市金明站";
            var result = ChsSpell.GetChsSpell(name);
            Assert.AreEqual(result, "KFSJMZ");
        }
    }
}
