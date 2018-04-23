namespace Kongrevsky.Utilities.Enum
{
    #region << Using >>

    using System;
    using Newtonsoft.Json.Serialization;

    #endregion

    public static class EnumUtils
    {
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