using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PonsUtil.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PonsUtil.Xml.Tests
{
    [TestClass()]
    public class XmlHelperTests
    {
        [TestMethod()]
        public void XmlDeserializeTest()
        {
            Gift g1 = new Gift { Level = 1, GiftName = "足球" };
            Gift g2 = new Gift { Level = 3, GiftName = "羽毛球" };
            List<Gift> gl = new List<Gift>() { g1, g2 };
            var people = new
            {
                Age = 18,
                Name = "Ann",
                Gifts = gl
            };

            string xml = XmlHelper.XmlSerialize(people, Encoding.UTF8);

            var p = XmlHelper.XmlDeserialize<People>(xml, Encoding.UTF8);
        }

        public class People
        {

            public int Age { get; set; }

            public string Name { get; set; }

            public List<Gift> Gifts { get; set; }
        }

        public class Gift
        {
            [XmlAttribute]
            public int Level { get; set; }
            [XmlElement]
            public string GiftName { get; set; }
        }
    }
}
