using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace PonsUtil.MefDI
{
    public class MefServiceLocator : MefServiceLocatorBase
    {
        private MefServiceLocator(ExportProvider provider)
            : base(provider)
        {
        }

        private static IServiceLocator _instance = CreateInstance();
        public static IServiceLocator Instance
        {
            get { return _instance; }
        }

        private static IServiceLocator CreateInstance()
        {
            var isHaveBin = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory).Any(s => s.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Equals("bin"));

            var container = isHaveBin ? MefHelper.CreateContainer("bin") : MefHelper.CreateContainer();

            var locator = new MefServiceLocator(container);

            return locator;
        }

        public override TService GetInstance<TService>()
        {
            try
            {
                return base.GetInstance<TService>();
            }
            catch (Exception ex)
            {
                //MailHelper.SendMailToUser("RunningFishSystemMef异常通知。", "GetInstance<TService>()" + ex.StackTrace, MailConfig.SystemMailAddress);
                _instance = CreateInstance();
            }

            return default(TService);
        }

        public override TService GetInstance<TService>(string key)
        {
            try
            {
                return base.GetInstance<TService>(key);
            }
            catch (Exception ex)
            {
                //MailHelper.SendMailToUser("RunningFishSystemMef异常通知key。", "GetInstance<TService>(string key)" + ex.StackTrace, MailConfig.SystemMailAddress);
                _instance = CreateInstance();
            }

            return default(TService);
        }
    }
}