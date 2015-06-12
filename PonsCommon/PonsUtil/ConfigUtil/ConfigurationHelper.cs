using System;
using System.Configuration;
using PonsUtil.Security;

namespace PonsUtil.ConfigUtil
{
    public static class ConfigurationHelper
    {
        public static string GetConnectionString(string key)
        {
            if (ConfigurationManager.ConnectionStrings[key] == null)
            {
                throw new Exception("配置文件有误");
            }
            return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings[key].ToString().Trim());
        }

        /// <summary>
        /// 取得配置信息,如果未配置则返回0
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            if (ConfigurationManager.AppSettings[key] == null)
            {
                return "0";
            }
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }

        /// <summary>
        /// 默认方式取得配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String GetAppSettingByDefault(string key)
        {
            if (ConfigurationManager.AppSettings[key] == null)
            {
                return null;
            }
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }

        /// <summary>
        /// 加密Config中的指定节
        /// </summary>
        /// <param name="sectionName"></param>
        public static void ProtectSection(string sectionName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection section = config.GetSection(sectionName);
            if (section != null && !section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                config.Save();
            }
        }

        /// <summary>
        /// 解密Config中的指定节
        /// </summary>
        /// <param name="sectionName"></param>
        public static void UnProtectSection(string sectionName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection section = config.GetSection(sectionName);
            if (section != null && section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                config.Save();
            }
        }

    }
}
