using System;
using System.Text.RegularExpressions;

namespace NettyFrame.Common
{
    public static class StringExtention
    {
        /// <summary>
        /// IPV4正则表达式(不带端口号)
        /// </summary>
        public const string InternetProtcolV4 = @"((2[0-4]\d|25[0-5]|[01]?\d?\d)\.){3}(2[0-4]\d|25[0-5]|[01]?\d?\d)";


        //todo：匹配全字符，验证字符串，验证IPV4
        /// <summary>
        /// 获得完全匹配的正则表达式
        /// </summary>
        public static string GetPerfectRegStr(string regStr)
        {
            if (string.IsNullOrEmpty(regStr)) return string.Empty;
            const char first = '^';
            const char last = '$';
            if (regStr[0] != first)
            {
                regStr = first + regStr;
            }
            if (regStr[regStr.Length - 1] != last)
            {
                regStr += last;
            }
            return regStr;
        }
        /// <summary>
        /// 验证字符串
        /// </summary>
        public static bool VerifyRegex(this string inputStr, string regStr)
        {
            return !string.IsNullOrEmpty(regStr) && !string.IsNullOrEmpty(inputStr) && Regex.IsMatch(inputStr, regStr);
        }
        /// <summary>
        /// 验证字符串
        /// </summary>
        public static bool VerifyRegex(this string inputStr, string regStr, bool isPerfect)
        {
            regStr = isPerfect ? GetPerfectRegStr(regStr) : regStr;
            return VerifyRegex(inputStr, regStr);
        }
        /// <summary>
        /// 验证字符串是否为IPv4
        /// </summary>
        public static bool IsIPv4(this string inputStr)
        {
            return VerifyRegex(inputStr, InternetProtcolV4, true);
        }
    }
}
