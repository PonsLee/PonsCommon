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
    public class CommonTests
    {
        List<string> list = new List<string> { "1", "2", "3" };
        [TestMethod()]
        public void GetOracleInParameterWhereSqlTest()
        {
            string sql = PonsUtil.Common.GetOracleInParameterWhereSql("waybillno", "123,4242,13131,24242,4242", true, false);
            string a = "abc";
            string b = a;
            a = "123";
            Console.WriteLine(a + b);

            List<string> listnew = new List<string>();
            Change(list);
            Console.WriteLine(listnew);

            List<string> listref = new List<string>();
            Change(list, ref listref);
            Console.WriteLine(listref);
        }

        private void Change(List<string> list)
        {
            if (list.Count > 0)
            {
                list = new List<string> { "4", "5" };
            }
        }

        private void Change(List<string> list, ref List<string> newlist)
        {
            if (list.Count > 0)
            {
                newlist = new List<string> { "4", "5" };
            }
        }
    }
}
