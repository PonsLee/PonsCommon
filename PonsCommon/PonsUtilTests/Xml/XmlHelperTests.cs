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
            Class2 c1 = new Class2 { IntValue = 3, StrValue = "Pons" };
            Class2 c2 = new Class2 { IntValue = 4, StrValue = "Abc 12345" };

            Class3 c3 = new Class3 { IntValue = 5, StrValue = "Test List" };

            Root root = new Root { Class3 = c3, List = new List<Class2> { c1, c2 } };

            string xmls = XmlHelper.XmlSerialize(root, Encoding.UTF8);
            Root r = XmlHelper.XmlDeserialize<Root>(xmls, Encoding.UTF8);

            Gift g1 = new Gift { Level = 1, GiftName = "足球" };
            Gift g2 = new Gift { Level = 3, GiftName = "羽毛球" };

            List<Gift> gl = new List<Gift>()
            {
                new Gift { Level = 3, GiftName = "羽毛球" },
                new Gift { Level = 1, GiftName = "足球" }
            };
            People people = new People
            {
                Age = 18,
                Name = "Ann",
                Gifts = new List<Gift>()
                {
                    new Gift { Level = 3, GiftName = "羽毛球" },
                    new Gift { Level = 1, GiftName = "足球" }
                }
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
            [XmlElement]
            public int Level { get; set; }
            [XmlElement]
            public string GiftName { get; set; }
        }

        public class Class1
        {
            public int IntValue { get; set; }

            public string StrValue { get; set; }
        }

        public class Class2
        {
            [XmlAttribute]
            public int IntValue { get; set; }

            [XmlElement]
            public string StrValue { get; set; }
        }

        public class Class3
        {
            [XmlAttribute]
            public int IntValue { get; set; }

            [XmlText]
            public string StrValue { get; set; }
        }

        [XmlType("c4")]
        public class Class4
        {
            [XmlAttribute("id")]
            public int IntValue { get; set; }

            [XmlElement("name")]
            public string StrValue { get; set; }
        }

        public class Root
        {
            public Class3 Class3 { get; set; }

            public List<Class2> List { get; set; }
        }
    }
}
