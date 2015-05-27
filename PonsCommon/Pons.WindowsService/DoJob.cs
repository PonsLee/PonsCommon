using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Pons.WindowsService
{
    public class DoJob : IStatefulJob
    {
        public void Execute(JobExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void Do()
        {

        }
    }
}
