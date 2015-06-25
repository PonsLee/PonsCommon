using System.Text.RegularExpressions;

namespace PonsUtil
{
    public class VerifyHelper
    {
        /// <summary>
        /// 特殊字符
        /// </summary>
        public const string SpecialChar = @"[~!@#$%&*':?/.\\|}{)(=]";

        /// <summary>
        /// 判断是否为正确的固定电话号码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsValidPhone(string phone)
        {
            //string pattern = @"^(\d{7,8}|(\(|\（)\d{3,4}(\)|\）)\d{7,8}|\d{3,4}(-?)\d{7,8})(((-|转)\d{1,9})?)$ ";
            if (string.IsNullOrEmpty(phone)) return false;
            const string pattern = @"\b\d{7,16}";
            //const string pattern = @"^(?< 国家代码>(\+86)|(\(\+86\)))?\D?(?<电话号码>(?<三位区号> ((010|021|022|023|024|025|026|027|028|029|852)|(\(010\)|\(021\)|\(022\)| \(023\)|\(024\)|\(025\)|\(026\)|\(027\)|\(028\)|\(029\)|\(852\)))\D? \d{8}|(?<四位区号>(0[3-9][1-9]{2})|(\(0[3-9][1-9]{2}\)))\D?\d{7,8})) (?<分机号>\D?\d{1,4})?$";
            return Regex.Match(phone, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// 判断是否为正确的手机号
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;
            const string pattern = @"\b1(3|5|8)\d{9}\b";
            //const string pattern = @"^(?<国家代码>(\+86)|(\(\+86\)))?(?<手机号>((13[0-9]{1})|(15[0-9]{1})|(18[0,5-9]{1}))+\d{8})$";
            return Regex.Match(mobile, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// 判断是否为正确的邮政编码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsValidPostalCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            const string pattern = @"\b\d{6}\b";
            return Regex.Match(code, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// 判断是否为正确的Email地址
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            const string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

            return Regex.Match(email, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// 是否中文字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCnString(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            const string pattern = @"[\u4e00-\uf900]";
            return Regex.Match(str, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// 是否是中文字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsCnString(char ch)
        {
            return IsCnString(ch.ToString());
        }
    }
}
