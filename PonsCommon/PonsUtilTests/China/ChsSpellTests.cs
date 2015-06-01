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

            //中文符号返回*
            var chars = "k！开，封,!01";
            var result1 = ChsSpell.GetChsSpell(chars);
            Assert.AreEqual(result1, "k*K*F,!01");
        }
    }
}
