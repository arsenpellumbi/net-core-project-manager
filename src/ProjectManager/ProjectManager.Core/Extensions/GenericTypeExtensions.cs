using System.Linq;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class GenericTypeExtensions
    {
        public static string GetGenericTypeName(this Type type)
        {
            type.ThrowIfNull(nameof(type));

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                return type.Name;
            }
        }

        /// <summary>
        /// get type name
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static string GetGenericTypeName(this object obj)
        {
            obj.ThrowIfNull(nameof(obj));

            return obj.GetType().GetGenericTypeName();
        }
    }
}