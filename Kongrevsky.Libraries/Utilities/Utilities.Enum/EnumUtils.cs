namespace Utilities.Enum
{
    #region << Using >>

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Serialization;

    #endregion

    public static class EnumUtils
    {
        /// <summary>
        /// Returns DisplayAttribute if exists either ToString()
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            var name = enumValue.ToString();
            var memberInfos = enumValue.GetType().GetMember(name);
            if (memberInfos.Any())
            {
                var displayAttribute = memberInfos.First().GetCustomAttribute<DisplayAttribute>();
                return displayAttribute?.GetName() ?? Regex.Replace(name, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Replace('_', ' ');
            }

            return name;
        }

        /// <summary>
        /// Returns Enum value
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetValue(this Enum enumValue)
        {
            var resolvedPropertyName = new CamelCasePropertyNamesContractResolver().GetResolvedPropertyName(enumValue.ToString());
            return resolvedPropertyName;
        }
    }
}