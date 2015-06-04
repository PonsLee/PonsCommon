using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonsUtil.Json;

namespace PonsUtilTests.Json
{
    [TestClass()]
    public class DynamicJsonConverterTests
    {
        [TestMethod()]
        public void DeserializeTest()
        {

            string json = "{\"seq\":\"12343433\",\"last_fetch_time\":\"2015-04-02 00:21:24\"}";
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(json, typeof(object));
            var a = obj.seq;
        }
    }
}
