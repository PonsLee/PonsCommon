using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pons.WindowsService
{
    [TestFixture]
    public class ServiceTest
    {
        [Test]
        public void TestDoJob()
        {
            DoJob job = new DoJob();
            job.Do();
        }
    }
}
