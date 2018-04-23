namespace Kongrevsky.Utilities.Object
{
    #region << Using >>

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    #endregion

    public static class ObjectUtils
    {
        /// <summary>
        /// Returns DisplayAttribute if exists either ToString()
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayName<TEnum>(this TEnum value)
        {
            var name = value.ToString();
            var memberInfos = value.GetType().GetMember(name);
            if (memberInfos.Any())
            {
                var displayAttribute = memberInfos.First().GetCustomAttribute<DisplayAttribute>();
                return displayAttribute?.GetName() ?? Regex.Replace(name, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
            }

            return name;
        }

        /// <summary>
        /// Converts Object to specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertToType<T>(this object obj)
        {
            if (obj is T variable)
                return variable;

            if (obj == null || obj is string && (string)obj == string.Empty)
                return default(T);

            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Tries to convert Object to specified type.
        /// Returns true if it is possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryConvertToType<T>(this object obj, out T result)
        {
            switch (obj)
            {
                case T variable:
                    result = variable;
                    return true;
                case null:
                    result = default(T);
                    return false;
            }

            try
            {
                var type = typeof(T);
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    result = (T)Convert.ChangeType(obj, type.GetGenericArguments()[0], CultureInfo.InvariantCulture);
                else
                    result = (T)Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);

                return true;
            }
            catch (InvalidCastException)
            {
                result = default(T);
                return false;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Tries to convert Object to specified type.
        /// Returns true if it is possible
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryConvertToType(this object obj, Type targetType, out object result)
        {
            if (targetType.IsInstanceOfType(obj))
            {
                result = obj;
                return true;
            }

            try
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    result = Convert.ChangeType(obj, targetType.GetGenericArguments()[0], CultureInfo.InvariantCulture);
                else if (targetType.IsEnum)
                    result = Enum.Parse(targetType, obj.ToString(), true);
                else
                    result = Convert.ChangeType(obj, targetType, CultureInfo.InvariantCulture);

                return true;
            }
            catch (InvalidCastException)
            {
                result = null;
                return false;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Returns value of the property by specified name
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetPropValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                    return null;

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                    return null;

                obj = info.GetValue(obj, null);
            }

            return obj;
        }

        /// <summary>
        /// Tries to set value of the property by specified name.
        /// Returns true if it is possible.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySetPropValue(this object obj, string name, object value)
        {
            var parts = name.Split('.');
            foreach (var part in parts)
            {
                if (obj == null)
                    return false;

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                    return false;

                if (part == parts.Last())
                    info.SetValue(obj, value, null);
            }

            return true;
        }

        /// <summary>
        /// Returns value of the property by specified name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetPropValue<T>(this object obj, string name)
        {
            var result = GetPropValue(obj, name);
            if (result == null)
                return default(T);

            // throws InvalidCastException if types are incompatible
            return (T)result;
        }

        /// <summary>
        /// Tries return value of the property by specified name.
        /// Returns true if it is possible, value is returned by out parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetPropValue<T>(this object obj, string name, out T result)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                {
                    result = default(T);
                    return false;
                }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                {
                    result = default(T);
                    return false;
                }

                obj = info.GetValue(obj, null);
            }

            result = (T)obj;
            return true;
        }

        /// <summary>
        /// Detects if type is nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType) return true;                        // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false;                                              // value-type
        }

        /// <summary>
        /// Detects if type is primitive, enum or one of the following types:
        /// string, decimal, DateTime
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsSimple(type.GetGenericArguments()[0]);

            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime);
        }
    }
}