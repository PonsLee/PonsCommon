using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.Security
{
    public class DES
    {
        private const string DES_KEY = "qF#e/E.yu> 0.JGuU$`p9hS%jwBnH=rm";
        /// <summary>
        /// 3des加密字符串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>加密后并经base63编码的字符串</returns>
        /// <remarks>重载，指定编码方式</remarks>
        public static string Encrypt3DES(string plainText, Encoding encoding)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;
            var DES = new
                TripleDESCryptoServiceProvider();
            var hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(encoding.GetBytes(DES_KEY));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = encoding.GetBytes(plainText);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock
                                              (Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// 3des加密字符串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>加密后并经base63编码的字符串</returns>
        public static string Encrypt3DES(string plainText)
        {
            return Encrypt3DES(plainText, Encoding.Default);
        }

        /// <summary>
        /// 3des解密字符串
        /// </summary>
        /// <param name="entryptText">密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt3DES(string entryptText)
        {
            return Decrypt3DES(entryptText, Encoding.Default);
        }

        /// <summary>
        /// 3des解密字符串
        /// </summary>
        /// <param name="entryptText">密文</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>解密后的字符串</returns>
        /// <remarks>静态方法，指定编码方式</remarks>
        public static string Decrypt3DES(string entryptText, Encoding encoding)
        {
            var DES = new
                TripleDESCryptoServiceProvider();
            var hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(encoding.GetBytes(DES_KEY));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result;
            try
            {
                byte[] Buffer = Convert.FromBase64String(entryptText);
                result = encoding.GetString(DESDecrypt.TransformFinalBlock
                                                (Buffer, 0, Buffer.Length));
            }
            catch (Exception e)
            {
                throw (new Exception("Invalid Key or input string is not a valid base64 string", e));
            }
            return result;
        }
    }
}
