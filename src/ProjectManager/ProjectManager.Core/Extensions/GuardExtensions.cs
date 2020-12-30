using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace System
{
    [DebuggerStepThrough]
    public static class GuardExtensions
    {
        /// <summary>
        /// throw exception if @object is null,
        /// </summary>
        /// <param name="value"></param>
        /// <param name="argument"></param>
        /// <exception cref="ArgumentNullException"/>
        public static void ThrowIfNullOrEmpty(this string value, string argument)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argument);
            }
        }

        /// <summary>
        /// throw exception if @object is null,
        /// </summary>
        /// <param name="value">value to be checked</param>
        /// <param name="argument"></param>
        /// <param name="message">exception message</param>
        /// <exception cref="ArgumentNullException"/>
        public static void ThrowIfNullOrEmpty(this string value, string argument, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argument, message);
            }
        }

        /// <summary>
        /// throw exception if @object is null,
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="argument"></param>
        /// <exception cref="ArgumentNullException"/>
        public static void ThrowIfNull(this object obj, string argument)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argument);
            }
        }

        /// <summary>
        /// throw exception if @object is null,
        /// </summary>
        /// <param name="object"></param>
        /// <param name="argument"></param>
        /// <exception cref="ArgumentNullException"/>
        public static void ThrowIfNull(this Guid value, string argument)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentNullException(string.Format("{0} must not be empty", argument));
            }
        }

        public static void ThrowIfNull(this object value, string argument, string message)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argument, message);
            }
        }

        public static void ThrowIfNotAllowed<T>(this T value, T notAllowedValue, string argument, string message)
            where T : struct
        {
            if (value.Equals(notAllowedValue))
            {
                throw new ArgumentException(argument, message);
            }
        }
    }
}