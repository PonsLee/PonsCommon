using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonsUtil.Json;
namespace PonsUtil.Json.Tests
{
    [TestClass()]
    public class JsonHelperTests
    {
        [TestMethod()]
        public void JsonToDynamicTest()
        {
            string json = "{\"seq\":\"12343433\",\"last_fetch_time\":\"2015-04-02 00:21:24\"}";
            dynamic obj = JsonHelper.JsonToDynamic(json);
            Assert.AreEqual("12343433", obj.seq);
            Assert.AreEqual("2015-04-02 00:21:24", obj.last_fetch_time);
        }
    }
}

namespace PonsUtilTests.Json
{
    [TestClass()]
    public class JsonHelperTests
    {
        [TestMethod()]
        public void ConvertJsonToDynamicTest()
        {
            var j = JsonHelper.ConvertToJosnString(new { seq = "1111", last_fetch_time="2223333"});
            dynamic obj1 = JsonHelper.ConvertJsonToDynamic(j);
            Assert.AreEqual(obj1.deset, null);
            Assert.AreEqual("1111", obj1.seq);
            Assert.AreEqual("2223333", obj1.last_fetch_time);


            string json = "{\"seq\":\"12343433\",\"last_fetch_time\":\"2015-04-02 00:21:24\"}";
            dynamic obj = JsonHelper.ConvertJsonToDynamic(json);
            Assert.AreEqual(obj.deset, null);
            Assert.AreEqual("12343433", obj.seq);
            Assert.AreEqual("2015-04-02 00:21:24", obj.last_fetch_time);
        }
    }
}
