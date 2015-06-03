using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
namespace PonsUtil.WebTool
{
    /// <summary>
    /// 网络工具类。
    /// </summary>
    public abstract class WebUtils
    {
        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.KeepAlive = true;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] postData = Encoding.UTF8.GetBytes(BuildPostData(parameters));

            int sendCount = 0;
            while (sendCount < 5)
            {
                try
                {
                    Stream reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    reqStream.Close();

                    HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch
                {
                    sendCount++;
                }
            }
            return @"<wlb><is_success>F</is_success><error>远程服务器返回错误或者操作超时</error></wlb>";
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildPostData(parameters);
                }
                else
                {
                    url = url + "?" + BuildPostData(parameters);
                }
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.KeepAlive = true;
            req.ContentType = "application/x-www-form-urlencoded";

            int sendCount = 0;
            while (sendCount != 5)
            {
                try
                {
                    HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch
                {
                    sendCount++;
                }
            }
            return @"<wlb><is_success>F</is_success><error>远程服务器返回错误或者操作超时</error></wlb>";
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 每次读取不大于512个字符，并写入字符串
                char[] buffer = new char[512];
                int readBytes = 0;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Append(buffer, 0, readBytes);
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildPostData(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(value);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 创建证书
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string CreateSign(Dictionary<string, string> dic, string appSecret)
        {
            var result = from pair in dic orderby pair.Key select pair;
            try
            {
                StringBuilder str = new StringBuilder(appSecret);
                foreach (KeyValuePair<string, string> key in result)
                {
                    if (!string.IsNullOrEmpty(key.Value.Trim()))
                    {
                        str.Append(key.Key);
                        str.Append(key.Value);
                    }
                }
                str.Append(appSecret);
                string ret = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str.ToString(), "MD5").ToUpper();
                return ret;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoPostForTaobao(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.KeepAlive = true;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] postData = Encoding.UTF8.GetBytes(BuildPostData(parameters));

            int sendCount = 0;
            while (sendCount < 5)
            {
                try
                {
                    Stream reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    reqStream.Close();

                    HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    sendCount++;
                }
            }
            return @"<wlb><is_success>F</is_success><error>远程服务器返回错误或者操作超时</error></wlb>";
        }
    }
}
