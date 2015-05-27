using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PonsUtil;
using Quartz;
using Quartz.Impl;

namespace Pons.WindowsService
{
    public class ServicesManager
    {
        IScheduler _sched = null;

        public void Start()
        {
            try
            {
                if (null == _sched)
                {
                    NameValueCollection properties = new NameValueCollection();
                    properties["quartz.scheduler.instanceName"] = "XmlConfiguredInstance";
                    properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                    properties["quartz.threadPool.threadCount"] = "2";
                    properties["quartz.threadPool.threadPriority"] = "Normal";
                    properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.JobInitializationPlugin, Quartz";
                    properties["quartz.plugin.xml.fileNames"] = "~/JobConfig.xml";

                    ISchedulerFactory sf = new StdSchedulerFactory(properties);
                    _sched = sf.GetScheduler();
                }

                _sched.Start();

                MessageCollector.Instance.Collect("default", "服务启动", true);
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect("default", ex.ToString(), true);
            }
        }

        public void Stop()
        {
            try
            {
                if (null != _sched)
                {
                    _sched.Shutdown(true);
                }

                MessageCollector.Instance.Collect("default", "服务停止", true);
            }
            catch (Exception ex)
            {
                MessageCollector.Instance.Collect("default", ex.ToString(), true);
            }
        }
    }
}
