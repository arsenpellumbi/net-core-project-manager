using System.Linq;
using ProjectManager.Core.SeedWork.Domain;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string str1, params string[] strings)
        {
            return strings?.All(str => string.Equals(str1, str, StringComparison.InvariantCultureIgnoreCase)) == true;
        }

        public static bool EqualsAnyIgnoreCase(this string str1, params string[] strings)
        {
            return strings?.Any(str => string.Equals(str1, str, StringComparison.InvariantCultureIgnoreCase)) == true;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(this string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            var pLen = postfix?.Length ?? 0;

            var result = str.Substring(0, maxLength - pLen);
            if (!string.IsNullOrEmpty(postfix))
            {
                result += postfix;
            }

            return result;
        }

        public static string FirstCharToUpper(this string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(this string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Ensures that a string is the specified enum
        /// </summary>
        /// <typeparam name="T">Input string</typeparam>
        /// <param name="value">Default enum value</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue)
            where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }

            throw new DomainException($"{typeof(T).Name} is not valid!");
        }
    }
}