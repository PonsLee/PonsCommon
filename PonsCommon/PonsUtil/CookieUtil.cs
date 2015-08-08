using System;
using System.Linq;
using System.Text;
using System.Web;
using PonsUtil.Security;

namespace PonsUtil
{
    public class CookieUtil
    {
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="CookieName">Cookie名称</param>
        /// <param name="CookieValue">Cookie值</param>
        /// <param name="ExpiresTime">有效时间</param>
        public static void AddCookie(string CookieName, string CookieValue, int ExpiresTime)
        {
            CookieValue = HttpContext.Current.Server.UrlEncode(CookieValue);
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                ClearCookie(CookieName);//如果存在则清空
            }

            var MyCookie = new HttpCookie(CookieName);

            MyCookie.Value = CookiesEncrypt.Encrypt(CookieValue);
            if (ExpiresTime != 0)
                MyCookie.Expires = DateTime.Now.AddHours(ExpiresTime);

            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }

        public static void AddCookie(string CookieName, string CookieValue, int ExpiresTime, string domain)
        {
            CookieValue = HttpContext.Current.Server.UrlEncode(CookieValue);
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                ClearCookie(CookieName);//如果存在则清空
            }

            var MyCookie = new HttpCookie(CookieName);

            MyCookie.Value = CookiesEncrypt.Encrypt(CookieValue);
            MyCookie.Domain = domain;
            if (ExpiresTime != 0)
                MyCookie.Expires = DateTime.Now.AddHours(ExpiresTime);

            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }

        /// <summary>
        /// 根据Cookie名称获取Cookie的值
        /// </summary>
        /// <param name="CookieName"></param>
        public static string GetCookie(string CookieName)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[CookieName] == null)
                {
                    return "";
                }

                return HttpContext.Current.Server.UrlDecode(CookiesEncrypt.Decrypt(HttpContext.Current.Request.Cookies[CookieName].Value));
            }
            catch (Exception ex)
            {
                throw new Exception("未找到该cookie值！" + CookieName + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清理Cookie
        /// </summary>
        public static void ClearCookie(string CookieName)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[CookieName] != null)
                {
                    HttpCookie mycookie = HttpContext.Current.Request.Cookies[CookieName];
                    var ts = new TimeSpan(0, 0, 0, 0); //时间跨度 
                    mycookie.Expires = DateTime.Now.Add(ts); //立即过期 
                    HttpContext.Current.Response.Cookies.Remove(CookieName); //清除 
                    HttpContext.Current.Response.Cookies.Add(mycookie); //写入立即过期的*/
                    HttpContext.Current.Response.Cookies[CookieName].Expires = DateTime.Now.AddDays(-1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作cookie！" + CookieName + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        public static bool ExistCookie(string cookieName)
        {
            return HttpContext.Current.Request.Cookies.AllKeys.ToList().Exists(item => item == cookieName);
        }

        public static void AddCookie<T>(string name, T value)
        {
            ClearCookie(name);
            AddCookie(name, value.ToString(), 1);
        }

        public static void AddCookie<T>(string name, T value, int expiresTime)
        {
            ClearCookie(name);

            AddCookie(name, value.ToString(), expiresTime);
        }

        public static T GetCookie<T>(string name)
        {
            try
            {
                return DataConvert.ToValue<T>(GetCookie(name));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        public static string GetLastCookie(HttpCookie cookies)
        {
            try
            {
                return HttpContext.Current.Server.UrlDecode(CookiesEncrypt.Decrypt(cookies.Value));
            }
            catch (Exception ex)
            {
                throw new Exception("未找到该cookie值！" + cookies.Name + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取客户端信息已经服务的错误信息
        /// </summary>
        /// <returns></returns>
        public static string GetHttpRequestAndError(Exception exception)
        {
            StringBuilder RequestAndErrorMsg = new StringBuilder();
            if (HttpContext.Current.Request.Cookies["RFDLMSUserID"] != null)
            {
                RequestAndErrorMsg.Append("访问时间：" + DateTime.Now + "<br>");
                RequestAndErrorMsg.Append("员工ID：" + "<br>");
                RequestAndErrorMsg.Append("员工名称：" + "<br>");
                RequestAndErrorMsg.Append("员工代号：" + "<br>");
                RequestAndErrorMsg.Append("所在部门ID：" + "<br>");
                RequestAndErrorMsg.Append("所在部门名称：" + "<br>");
                RequestAndErrorMsg.Append("配送商编号：" + "<br>");
            }
            string user_IP = "";
            //HttpContext.Current.Request.
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                user_IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                user_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }

            RequestAndErrorMsg.Append("客户端IP：" + user_IP + "<br>");
            RequestAndErrorMsg.Append("客户端DNS主机名：" + HttpContext.Current.Request.UserHostName + "<br>");
            RequestAndErrorMsg.Append("客户端使用平台：" + HttpContext.Current.Request.Browser.Platform + "<br>");
            RequestAndErrorMsg.Append("客户端使用浏览器：" + HttpContext.Current.Request.Browser.Type + "<br>");
            RequestAndErrorMsg.Append("客户端浏览器版本号：" + HttpContext.Current.Request.Browser.Version + "<br>");
            RequestAndErrorMsg.Append("客户端请求URL：" + HttpContext.Current.Request.Url + "<br>");
            RequestAndErrorMsg.Append(string.Format("错误页面：{0}\r\nMessage:{1}\r\nSource:{2}\r\nTrace:{3}",
                                                    HttpContext.Current.Request.Url, exception.Message, exception.Source, exception.StackTrace));
            return RequestAndErrorMsg.ToString();
        }
    }
}
