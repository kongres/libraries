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
            return type.GetProperties().FirstOrDefault(x => string.Equals(x.Name, name, isCaseIgnore ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
        }
    }
}