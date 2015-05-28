using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.Security
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class MD5
    {
        /// <summary>
        /// 字符串MD5加密，返回大写字母
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>密文（大写字母）</returns>
        public static string Encrypt(string plainText)
        {
            MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
            byte[] b = md.ComputeHash(Encoding.Default.GetBytes(plainText));
            string s = string.Empty;
            for (int i = 0; i < b.Length; i++)
            {
                s += (b[i].ToString("x2"));
            }
            return s.ToUpper();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static String GetMD5Str(String info)
        {
            try
            {
                byte[] res = System.Text.Encoding.Default.GetBytes(info);
                MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
                byte[] result = md.ComputeHash(res);
                byte[] hash = md.ComputeHash(result);
                StringBuilder sbuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    int v = hash[i] & 0xFF;
                    if (v < 16) sbuilder.Append("0");
                    sbuilder.Append(Convert.ToString(v, 16).ToUpper());
                }
                return sbuilder.ToString();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// TMALL MD5加密算法
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string Md532(string Str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Str.Trim(), "md5").ToLower();
        }
    }
}
