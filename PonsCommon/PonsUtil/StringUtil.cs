using System.Text.RegularExpressions;
using System.Web;
using System;
using System.Data;

namespace PonsUtil
{
    public static class StringUtil
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

        /// <summary>
        /// 是否是正确的订单号
        /// </summary>
        /// <param name="formcode"></param>
        /// <returns></returns>
        public static bool IsValidFormCode(string formcode)
        {
            if (string.IsNullOrEmpty(formcode)) return false;
            const string pattern = @"^\b(1|2|3|4|5|7|8|9)\d{11,14}\b$";
            return Regex.Match(formcode, pattern, RegexOptions.Compiled).Success;
        }

        /// <summary>
        /// 是否包含特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsContainSpecialChar(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return Regex.Match(str, SpecialChar, RegexOptions.Compiled).Success;
        }

        public static void Alert(string Message)
        {
            string js = @"<Script language='JavaScript'>
                            alert('" + Message + "');</Script>";

            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1
                = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");
            return reg1.IsMatch(str);
        }
        /// <summary>
        /// 是否是数字或字母以及下划线组成
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumericOrLetter(string str)
        {
            System.Text.RegularExpressions.Regex reg1
               = new System.Text.RegularExpressions.Regex(@"^\w+$");
            return reg1.IsMatch(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutString(string str, int length)
        {
            string newString = string.Empty;
            if (str != string.Empty)
            {
                if (str.Length > length)
                {
                    newString = str.Substring(0, length);
                }
                else
                {
                    newString = str;
                }
            }
            return newString;
        }
        /// <summary>
        /// 判断是否为空数据
        /// </summary>
        /// <typeparam name="T">当前类型</typeparam>
        /// <param name="current">当前对象</param>
        /// <returns></returns>
        public static bool IsNullData<T>(this T current)
        {
            if (current == null || (object)current == DBNull.Value) return true;
            return String.IsNullOrEmpty(current.ToString().Trim());
        }
        /// <summary>
        /// 格式化货币金额输出
        /// </summary>
        /// <param name="money">当前金额</param>
        /// <returns></returns>
        public static string FormatMoney(object money)
        {
            if (money.IsNullData()) return string.Empty;
            return Convert.ToDecimal(money.ToString()).ToString("#,##0.###");
        }

        /// <summary>
        /// 格式化时间输出
        /// </summary>
        /// <param name="date">当前时间</param>
        /// <param name="datepart">选择提取部分</param>
        /// <example>sd:短日期；st:短日期；ld:长时间；lt:短时间; all: 整个日期时间</example>
        /// <returns></returns>
        public static string FormatDateTime(object date, string datepart)
        {
            var result = DateTime.Now;
            if (date.IsNullData() || !DateTime.TryParse(date.ToString(), out result))
                return string.Empty;
            var datetime = Convert.ToDateTime(date.ToString().Trim());
            switch (datepart.ToLower())
            {
                case "":
                case "sd":
                    return datetime.ToShortDateString();
                case "st":
                    return datetime.ToShortTimeString();
                case "all":
                    return datetime.ToString();
                case "ld":
                    return datetime.ToLongDateString();
                case "lt":
                    return datetime.ToLongTimeString();
                default:
                    return datetime.ToString(datepart);
            }
        }
        /// <summary>
        /// 格式化时间输出(默认格式化为长日期)
        /// </summary>
        /// <param name="date">当前时间</param>
        /// <returns></returns>
        public static string FormatDateTime(object date)
        {
            return FormatDateTime(date, "ld");
        }

        /// <summary>
        /// 格式化时间输出(默认格式化为长日期)
        /// </summary>
        /// <param name="date">当前时间</param>
        /// <returns></returns>
        public static DateTime FormatDateTime(object date, DateTime defaultValue)
        {
            var datetime = FormatDateTime(date);
            return datetime.IsNullData() ? defaultValue : Convert.ToDateTime(datetime);
        }

        public static string FormatDataRow(DataRow dr, string field)
        {
            return dr[field].IsNullData() ? "" : dr[field].ToString().Trim();
        }

        /// <summary>
        /// 判断是否为空行
        /// </summary>
        /// <param name="dr">当前的数据行</param>
        /// <returns></returns>
        public static bool IsEmptyDataRow(DataRow dr)
        {
            var result = true;
            var items = dr.ItemArray;
            foreach (var item in items)
            {
                if (!item.IsNullData())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// 判断当前行是否为模板对应行
        /// </summary>
        /// <param name="dr">当前行</param>
        /// <param name="template">模板</param>
        /// <param name="index">模板行索引</param>
        /// <returns></returns>
        public static bool IsTemplateRow(this DataRow dr, DataTable template, int index)
        {
            var result = false;
            var items = dr.ItemArray;
            for (var i = 0; i < template.Columns.Count; i++)
            {
                var dc = template.Columns[i];
                var cell = items[i].IsNullData() ? "" : items[i].ToString().Trim();
                var col = FormatDataRow(template.Rows[index], dc.ColumnName);
                if (cell != col)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 将当前字符串按照指定的分隔符进行叠加
        /// </summary>
        /// <param name="current">当前字符串</param>
        /// <param name="appendStr">需要叠加的字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string Append(this string current, string appendStr, char separator)
        {
            current += current.IsNullData() ? appendStr.Trim() : separator + appendStr.Trim();
            return current;
        }
        /// <summary>
        /// 将当前字符串按照指定的分隔符进行叠加（默认按空格叠加）
        /// </summary>
        /// <param name="current">当前字符串</param>
        /// <param name="appendStr">需要叠加的字符串</param>
        /// <returns></returns>
        public static string Append(this string current, string appendStr)
        {
            current += Append(current, appendStr, ' ');
            return current;
        }
        /// <summary>
        /// 将当前对象转化为整数(未转化成功则取默认值)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ConvertToInt<T>(this T current, int defaultValue)
        {
            var result = 0;
            return current.IsNullData() || !int.TryParse(current.ToString(), out result) ? defaultValue : result;
        }

        /// <summary>
        /// 将当前对象转化为整数(未转化成功则取默认值)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long ConvertToLong<T>(this T current, long defaultValue)
        {
            long result = 0;
            return current.IsNullData() || !long.TryParse(current.ToString(), out result) ? defaultValue : result;
        }

        /// <summary>
        /// 将当前对象转化为布尔(未转化成功则取默认值)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool ConvertToBool<T>(this T current, bool defaultValue)
        {
            var result = false;
            return current.IsNullData() || !bool.TryParse(current.ToString(), out result) ? defaultValue : result;
        }

        /// <summary>
        /// 将当前对象转化为布尔(未转化成功则取false)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <returns></returns>
        public static bool ConvertToBool<T>(this T current)
        {
            return current.ConvertToBool(false);
        }

        /// <summary>
        /// 将当前对象转化为整数(未转化成功则取0)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <returns></returns>
        public static int ConvertToInt<T>(this T current)
        {
            return current.ConvertToInt(0);
        }

        /// <summary>
        /// 将当前对象转化为整数(未转化成功则取0)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <returns></returns>
        public static long ConvertToLong<T>(this T current)
        {
            return current.ConvertToLong(0);
        }

        /// <summary>
        /// 将当前对象转化为小数(未转化成功则取默认值)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal<T>(this T current, decimal defaultValue)
        {
            var result = 0.0m;
            return current.IsNullData() || !decimal.TryParse(current.ToString(), out result) ? defaultValue : result;
        }
        /// <summary>
        /// 将当前对象转化为小数(未转化成功则取0)
        /// </summary>
        /// <param name="current">当前对象</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal<T>(this T current)
        {
            return current.ConvertToDecimal(0m);
        }

        /// <summary>
        /// 根据字符分割字符串，取字符串的前半段还是后半段
        /// </summary>
        /// <param name="strValue">原始字符串</param>
        /// <param name="splitChar">用了分割的字符</param>
        /// <param name="isGetLeft">取分割完字符的左面还是右面</param>
        /// <returns>分割后的结果</returns>
        public static string SubString(string strValue, char splitChar, bool isGetLeft)
        {
            if (isGetLeft == true) return strValue.Substring(0, strValue.LastIndexOf(splitChar));

            return strValue.Substring(strValue.LastIndexOf(splitChar) + 1);
        }
        /// <summary>
        /// 时间加24小时
        /// </summary>
        /// <param name="stime"></param>
        /// <returns></returns>
        public static string HourAdd24(string stime)
        {
            string[] arr1 = (stime.Split(':'));
            int changHour = Convert.ToInt32(arr1[0]) + 24;
            arr1[0] = changHour.ToString();
            return arr1[0] + ":" + arr1[1];
        }
        /// <summary>
        /// 比较两个时间字符串的大小，格式如：5:45
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static int CompareTime(string time1, string time2)
        {
            int i = -1;

            string[] arr1 = (time1.Split(':'));
            string[] arr2 = (time2.Split(':'));
            try
            {
                if (Convert.ToInt32(arr1[0]) > Convert.ToInt32(arr2[0]))
                {
                    i = 1;
                }
                else if (Convert.ToInt32(arr1[0]) < Convert.ToInt32(arr2[0]))
                {
                    i = 2;
                }
                else if (Convert.ToInt32(arr1[0]) == Convert.ToInt32(arr2[0]))
                {
                    if (Convert.ToInt32(arr1[1]) > Convert.ToInt32(arr2[1]))
                    {
                        i = 1;
                    }
                    else if (Convert.ToInt32(arr1[1]) < Convert.ToInt32(arr2[1]))
                    {
                        i = 2;
                    }
                    else
                    {
                        i = 0;
                    }
                }
                return i;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static string GetLongDate(string date, bool isEnd)
        {
            if (date.IsNullData()) return String.Empty;
            var year = date.Substring(0, 4).ConvertToInt();
            var month = date.Substring(date.Length - 2, 2);
            var seperate = date.Substring(date.Length - 3, 1);
            if (!isEnd) return date + seperate + "01";
            switch (month)
            {
                case "01":
                case "03":
                case "05":
                case "07":
                case "08":
                case "10":
                case "12":
                    return date + seperate + "31";
                case "02":
                    var isReap = ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0);
                    return date + seperate + (isReap ? "29" : "28");
                case "04":
                case "06":
                case "09":
                case "11":
                    return date + seperate + "30";
            }
            return String.Empty;
        }
        //// <summary>   
        /// 半角转全角的函数(SBC case)   
        /// </summary>   
        /// <param name=”input”>任意字符串</param>   
        /// <returns>全角字符串</returns>   
        ///<remarks>   
        ///全角空格为12288，半角空格为32   
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248   
        ///</remarks>   
        public static string ToSBC(string input)
        {
            //半角转全角：   
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>   
        /// 全角转半角的函数(DBC case)   
        /// </summary>   
        /// <param name=”input”>任意字符串</param>   
        /// <returns>半角字符串</returns>   
        ///<remarks>   
        ///全角空格为12288，半角空格为32   
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248   
        ///</remarks>   
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
    }
}
