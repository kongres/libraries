namespace Utilities.Enum
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Serialization;

    public static class EnumUtils
    {
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

        public static string GetValue(this Enum enumValue)
        {
            var resolvedPropertyName = new CamelCasePropertyNamesContractResolver().GetResolvedPropertyName(enumValue.ToString());
            return resolvedPropertyName;
        }

        

       
    }
}