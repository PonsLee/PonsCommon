using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PonsUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PonsUtil.Tests
{
    [TestClass()]
    public class OpenXMLHelperTests
    {
        [TestMethod()]
        public void OpenXMLHelperTest()
        {
            MemoryStream stream = new MemoryStream();
            string[,] array = new string[,] {{"123"},{"4242"}};
            using (OpenXMLHelper helper = new OpenXMLHelper(stream, OpenExcelMode.CreateNew))
            {
                helper.CreateNewWorksheet("信息导入模版");
                helper.WriteData(array);
                helper.Save();
            }
            stream.Seek(0, 0);
            //return new FileStreamResult(stream, "application/vnd.ms-excel") { FileDownloadName = HttpUtility.UrlPathEncode(fileName) };
        
        }

        [TestMethod()]
        public void OpenXMLHelperTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ReadUsedRangeToEndTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ReadUsedRangeToEndWithoutBlankTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void CreateNewWorksheetTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SaveTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SetRangeFontColorTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void WriteDataTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void WriteDataTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            throw new NotImplementedException();
        }
    }
}
