namespace Kongrevsky.Utilities.Reflection
{
    #region << Using >>

    using System;
    using System.Linq;
    using System.Reflection;

    #endregion

    public static class TypeUtils
    {
        /// <summary>
        /// Returns property by specified name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="isCaseIgnore"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyByName(this Type type, string name, bool isCaseIgnore = true)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (name.Contains("."))
            {
                var split = name.Split(new char[] { '.' }, 2);
                var property = GetPropertyByName(type, split[0], isCaseIgnore);
                if (property == null)
                    return null;
                return GetPropertyByName(property.PropertyType, split[1], isCaseIgnore);
            }
            else
            {
                return type.GetProperties().FirstOrDefault(x => string.Equals(x.Name, name, isCaseIgnore ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
            }
        }
    }
}