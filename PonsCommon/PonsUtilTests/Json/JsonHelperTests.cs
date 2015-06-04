using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonsUtil.Json;

namespace PonsUtilTests.Json
{
    [TestClass()]
    public class JsonHelperTests
    {
        [TestMethod()]
        public void ConvertJsonToDynamicTest()
        {
            string json = "{\"seq\":\"12343433\",\"last_fetch_time\":\"2015-04-02 00:21:24\"}";
            dynamic obj = JsonHelper.ConvertJsonToDynamic(json);
            var a = obj.deset;
            Assert.AreEqual(a, null);
            Assert.AreEqual("12343433", obj.seq);
            Assert.AreEqual("2015-04-02 00:21:24", obj.last_fetch_time);
        }
    }
}
