using System;
using System.Linq;

namespace Utilities.Object
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class ObjectUtils
    {
        public static string GetDescription<TEnum>(this TEnum value)
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

        public static T ConvertType<T>(this object obj)
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

        public static bool TryConvertType<T>(this object obj, out T result)
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

        public static bool TryConvertType(this object obj, Type targetType, out object result)
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

        public static object GetPropValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null) { return null; }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static bool TrySetPropValue(this object obj, string name, object value)
        {
            var parts = name.Split('.');
            foreach (var part in parts)
            {
                if (obj == null) { return false; }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return false; }

                if (part == parts.Last())
                    info.SetValue(obj, value, null);
            }
            return true;
        }

        public static T GetPropValue<T>(this object obj, string name)
        {
            var retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }

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

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type == typeof(string)
              || type == typeof(decimal)
              || type == typeof(DateTime);
        }
    }

}
