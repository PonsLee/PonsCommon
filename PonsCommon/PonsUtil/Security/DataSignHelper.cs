using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PonsUtil.Security
{
    public class DataSignHelper
    {
        /// <summary>
        /// 对MD5加密后的密文进行签名
        /// </summary>
        /// <param name="p_strKeyPrivate">私钥</param>
        /// <param name="m_strHashbyteSignature">MD5加密后的密文</param>
        /// <returns></returns>
        public static string SignData(string privateKey, string signString)
        {
            byte[] rgbHash = Convert.FromBase64String(signString);

            RSACryptoServiceProvider key = new RSACryptoServiceProvider();

            key.FromXmlString(privateKey);

            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);

            formatter.SetHashAlgorithm("MD5");

            byte[] inArray = formatter.CreateSignature(rgbHash);

            return Convert.ToBase64String(inArray);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="p_strKeyPublic">公钥</param>
        /// <param name="p_strHashbyteDeformatter">待验证的用户名</param>
        /// <param name="p_strDeformatterData">注册码</param>
        /// <returns></returns>
        public static bool VerifySignData(string publicKey, string signString, string p_strDeformatterData)
        {
            try
            {
                byte[] rgbHash = Convert.FromBase64String(signString);

                RSACryptoServiceProvider key = new RSACryptoServiceProvider();

                key.FromXmlString(publicKey);

                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);

                deformatter.SetHashAlgorithm("MD5");

                byte[] rgbSignature = Convert.FromBase64String(p_strDeformatterData);

                if (deformatter.VerifySignature(rgbHash, rgbSignature)) return true;

                return false;

            }
            catch
            {
                return false;
            }
        }

        public static string GetHash(string m_strSource)
        {
            HashAlgorithm algorithm = HashAlgorithm.Create("MD5");

            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);

            byte[] inArray = algorithm.ComputeHash(bytes);

            return Convert.ToBase64String(inArray);
        }

        /// <summary>
        /// SHA1 算法签名
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="signString">明文MD5加密转Base64编码</param>
        /// <returns>签名数据</returns>
        public static string SignDataSH(string privateKey, string signString)
        {
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            SHA1 sh = new SHA1CryptoServiceProvider();
            key.FromXmlString(privateKey);
            byte[] signData = key.SignData(Encoding.UTF8.GetBytes(signString), sh);
            return Convert.ToBase64String(signData);

        }
        /// <summary>
        /// SHA1 算法签名验证
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="signString">签名数据（签名后Base64编码）</param>
        /// <param name="p_strDeformatterData">明文MD5加密转Base64编码</param>
        /// <returns></returns>
        public static bool VerifySignDataSH(string publicKey, string signString, string p_strDeformatterData)
        {
            try
            {
                byte[] rgbHash = Convert.FromBase64String(p_strDeformatterData);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(publicKey);
                SHA1 sh = new SHA1CryptoServiceProvider();

                return key.VerifyData(Encoding.UTF8.GetBytes(signString), sh, rgbHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
