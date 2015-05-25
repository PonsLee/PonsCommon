using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace PonsUtil
{
    public class MessageCollector
    {
        private string _ip = string.Empty;
        private static MessageCollector _instance = null;
        private const string MailContentDelimiter = "<br/>*****************************************************<br/>";


        public static MessageCollector Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new MessageCollector();
                }
                return _instance;
            }
        }

        private MessageCollector()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void HandleMessage(string key, string msg, bool needNotice)
        {
            LogManager.GetLogger(key).Error(msg);
            if (needNotice)
            {
                LogManager.GetLogger("defaultEmail").Error(string.Format("IP:[{0}]{2}<br/>{2} {1}<br/>", GetIP, msg, MailContentDelimiter));
            }
        }

        public string GetIP
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_ip))
                    {
                        string strHostName = Dns.GetHostName(); //得到本机的主机名
                        IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
                        _ip = ipEntry.AddressList[0].ToString(); //假设本地主机为单网卡
                    }
                }
                catch
                {
                    _ip = "未取得IP地址";
                }

                return _ip;
            }
        }

        /// <summary>
        /// Write d own logs
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="msg">Message</param>
        public void Collect(Type type, string msg)
        {
            HandleMessage(type.FullName, msg, false);
        }

        /// <summary>
        /// Write down logs
        /// </summary>
        /// <param name="key">Key words</param>
        /// <param name="msg">Message</param>
        public void Collect(string key, string msg)
        {
            HandleMessage(key, msg, false);
        }

        /// <summary>
        /// Write down logs
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="msg">Message</param>
        /// <param name="needNotice">Whether notification</param>
        public void Collect(Type type, string msg, bool needNotice)
        {
            HandleMessage(type.FullName, msg, needNotice);
        }

        /// <summary>
        /// Write down logs
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="ex">Exception</param>
        /// <param name="needNotice">Whether notification</param>
        public void Collect(Type type, Exception ex, bool needNotice)
        {
            var msg = string.Format("Message:<br/>{0}{3}<br/>StackTrace:<br/>{1}{3}<br/>InnerException:{2}", ex.Message, ex.StackTrace, (ex.InnerException == null ? "" : ex.InnerException.Message), MailContentDelimiter);
            HandleMessage(type.FullName, msg, needNotice);
        }

        /// <summary>
        /// Write down logs
        /// </summary>
        /// <param name="key">Key words</param>
        /// <param name="msg">Message</param>
        /// <param name="needNotice">Whether notification</param>
        public void Collect(string key, string msg, bool needNotice)
        {
            HandleMessage(key, msg, needNotice);
        }
    }
}
