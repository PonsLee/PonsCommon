using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.EnumUntil
{
    public class EnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(System.Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumStr"></param>
        /// <returns></returns>
        public static string GetDescription<T>(string enumStr)
        {
            Type type = typeof(T);
            return GetDescription(type, enumStr);
        }

        public static string GetDescription<T>(object enumItem)
        {
            Type type = typeof(T);
            return GetDescription(type, enumItem.ToString());
        }

        /// <summary>
        /// 根据枚举取得取得Description特性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static string GetDescription<T>(T enumItem)
        {
            if (enumItem == null)
            {
                return string.Empty;
            }
            Type type = typeof(T);
            return GetDescription(type, enumItem.ToString());
        }

        public static string GetDescription<T>(int enumValue)
        {
            string descriptions = string.Empty;
            Type type = typeof(T);
            if (type.GetCustomAttributes(false)[0] is System.FlagsAttribute)
            {
                Array enumValues = System.Enum.GetValues(type);
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if ((Convert.ToInt32(enumValues.GetValue(i)) & enumValue) == Convert.ToInt32(enumValues.GetValue(i)))
                    {
                        if (descriptions != string.Empty)
                            descriptions += ";";
                        descriptions += GetDescription(type, System.Enum.GetNames(type)[i]);
                    }
                }
            }

            return descriptions;
        }

        /// <summary>
        /// 取得枚举项的描述
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="enumValue">枚举数值</param>
        /// <returns></returns>
        public static string GetDescription(Type type, int enumValue)
        {
            string descriptions = string.Empty;
            var custAttr = type.GetCustomAttributes(typeof(FlagsAttribute), false);
            if (custAttr != null && custAttr.Length > 0)
            {
                Array enumValues = System.Enum.GetValues(type);
                String[] enumNames = System.Enum.GetNames(type);
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if ((Convert.ToInt32(enumValues.GetValue(i)) & enumValue) == Convert.ToInt32(enumValues.GetValue(i)))
                    {
                        if (descriptions != string.Empty)
                            descriptions += ";";
                        descriptions += GetDescription(type, enumNames[i]);
                    }
                }
                return descriptions;
            }
            return GetDescription(type, System.Enum.GetName(type, enumValue));
        }

        /// <summary>
        /// 根据枚举字符取得Description特性的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="enumStr"></param>
        /// <returns></returns>
        public static string GetDescription(Type type, string enumStr)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("传入的值必须是枚举类型。");
            }
            MemberInfo[] memInfos = type.GetMembers();
            MemberInfo memInfo = null;
            string temp = string.Empty;
            for (int i = 0; i < memInfos.Length; i++)
            {
                memInfo = memInfos[i];
                if (string.Compare(enumStr, memInfo.Name, true) == 0)
                {
                    object[] attrs = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        temp = ((DescriptionAttribute)attrs[0]).Description;
                    }
                    return temp;
                }
            }
            return "";
            //throw new Exception("未知的枚举描述");
        }

        public static T GetValue<T>(string description)
        {
            Type type = typeof(T);
            MemberInfo[] memInfos = type.GetMembers();
            MemberInfo memInfo = null;
            string temp = "";
            for (int i = 0; i < memInfos.Length; i++)
            {
                memInfo = memInfos[i];
                object[] attrs = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    temp = ((DescriptionAttribute)attrs[0]).Description;
                    if (temp == description)
                    {
                        return (T)System.Enum.Parse(typeof(T), memInfo.Name, true);
                    }
                }
            }
            throw new Exception("未知的枚举描述");
        }

        /// <summary>
        /// 根据Name取得描述值
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEnumValue(FieldInfo[] fields, string name)
        {
            foreach (var item in fields)
            {
                if (item.Name.Equals(name))
                {
                    object[] custAttrs = item.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (custAttrs != null && custAttrs.Length > 0)
                    {
                        return ((DescriptionAttribute)custAttrs[0]).Description;
                    }
                }
            }
            return name;
        }

        /// <summary>
        /// 根据描述信息取得Value
        /// 使用DescriptionAttribute自定义属性
        /// Enum按照从小到大正数排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static int GetEnumValue<T>(string description)
        {
            Dictionary<int, string> dict = GetEnumValueAndDescriptions<T>();
            if (dict != null && dict.Count > 0)
            {
                List<int> keys = dict.Keys.ToList();
                List<string> values = dict.Values.ToList();
                for (int i = 0; i < dict.Count; i++)
                {
                    if (String.Compare(description, values[i], true) == 0)
                    {
                        return keys[i];
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 根据描述信息取得Value
        /// 使用DescriptionAttribute自定义属性
        /// Enum可以乱序排序并且可以包含负数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static int GetEnumValueEx<T>(string description)
        {
            Dictionary<int, string> dict = GetEnumValueAndDescriptionsEx<T>();
            if (dict != null && dict.Count > 0)
            {
                List<int> keys = dict.Keys.ToList();
                List<string> values = dict.Values.ToList();
                for (int i = 0; i < dict.Count; i++)
                {
                    if (String.Compare(description, values[i], true) == 0)
                    {
                        return keys[i];
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取枚举字典信息
        /// 需要按照正整数枚举从小到大排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumValueAndDescriptions<T>()
        {
            var rValue = new Dictionary<int, string>();
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            string[] names = System.Enum.GetNames(typeof(T));
            int[] values = System.Enum.GetValues(typeof(T)) as int[];
            if (fields.Length == names.Length && fields.Length == values.Length)
            {
                int intCnt = 0;
                foreach (object value in values)
                {
                    object[] attrs = fields[intCnt].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string description = string.Empty;
                    if (attrs != null && attrs.Length > 0)
                    {
                        description = ((DescriptionAttribute)attrs[0]).Description;
                    }
                    else
                    {
                        description = names[intCnt];
                    }
                    if (!rValue.ContainsKey((int)value))
                    {
                        rValue.Add((int)value, description);
                    }
                    intCnt += 1;
                }
            }
            return rValue;
        }

        /// <summary>
        /// 获取枚举字典信息
        /// 处理乱序或者包含负数的Enum顺序问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumValueAndDescriptionsEx<T>()
        {
            var rValue = new Dictionary<int, string>();
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            string[] names = System.Enum.GetNames(typeof(T));
            int[] values = System.Enum.GetValues(typeof(T)) as int[];
            if (fields.Length == names.Length && fields.Length == values.Length)
            {
                string desc;
                for (int i = 0; i < names.Length; i++)
                {
                    desc = GetEnumValue(fields, names[i]);
                    if (!rValue.ContainsKey(values[i]))
                    {
                        rValue.Add(values[i], desc);
                    }
                }
            }
            return rValue;
        }
    }
}
