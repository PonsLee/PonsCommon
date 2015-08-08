using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PonsUtil.Security
{
    public class CookiesEncrypt
    {
        /// <summary>
        /// 当前程序加密所使用的密钥8位
        /// </summary>
        private const string DES_KEY = "AR*@!*$)";

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="pToEncrypt">需要加密字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string pToEncrypt)
        {
            return Encrypt(pToEncrypt, Encoding.UTF8);
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="pToEncrypt">需要加密字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string pToEncrypt, Encoding encoding)
        {
            try
            {
                var des = new DESCryptoServiceProvider();
                byte[] inputByteArray = encoding.GetBytes(pToEncrypt);
                //建立加密对象的密钥和偏移量
                des.Key = ASCIIEncoding.ASCII.GetBytes(DES_KEY);
                des.IV = ASCIIEncoding.ASCII.GetBytes(DES_KEY);

                ICryptoTransform desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(inputByteArray, 0, inputByteArray.Length);
                return BitConverter.ToString(result);
            }
            catch (Exception ex)
            {
                throw (new Exception("Invalid Key or input string is not a valid string", ex));
            }
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="sKey">密匙</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string pToDecrypt)
        {
            return Decrypt(pToDecrypt, Encoding.UTF8);
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="sKey">密匙</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string pToDecrypt, Encoding encoding)
        {
            try
            {
                var DES = new DESCryptoServiceProvider();
                string[] sInput = pToDecrypt.Split("- ".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                DES.Key = ASCIIEncoding.ASCII.GetBytes(DES_KEY);
                DES.IV = ASCIIEncoding.ASCII.GetBytes(DES_KEY);
                ICryptoTransform desencrypt = DES.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return encoding.GetString(result);
            }
            catch (Exception ex)
            {
                throw (new Exception("Invalid Key or input string is not a valid string", ex));
            }
        }
    }
}
