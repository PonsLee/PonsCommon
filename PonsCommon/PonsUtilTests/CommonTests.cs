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
        List<string> lista = new List<string> { "1", "2", "3" };
        List<string> listb = new List<string> { "4", "5" };
        [TestMethod()]
        public void GetOracleInParameterWhereSqlTest()
        {
            string sql = PonsUtil.Common.GetOracleInParameterWhereSql("waybillno", "123,4242,13131,24242,4242", true, false);
            string a = "abc";
            string b = a;
            a = "123";
            Console.WriteLine(a + b);
            var listtemp = listb;
            Change(lista);
            Assert.AreNotSame(listtemp, lista);

            List<string> listref = new List<string>();
            Change(lista, ref listref);
            Assert.AreSame(listtemp, listref);

        }

        private void Change(List<string> list)
        {
            if (list.Count > 0)
            {
                list = listb;
            }
        }

        private void Change(List<string> list, ref List<string> newlist)
        {
            if (list.Count > 0)
            {
                newlist = listb;
            }
        }
    }
}
