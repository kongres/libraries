namespace Kongrevsky.Infrastructure.Web.ActionLogger.Attributes
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.Web.ActionLogger.Models;

    #endregion

    public class LogPropertyAttribute : Attribute
    {
        public LogPropertyAttribute(LogPropertyType propertyType)
        {
            PropertyType = propertyType;
        }

        public LogPropertyAttribute(LogPropertyType propertyType, string propertyName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
        }

        public LogPropertyType PropertyType { get; }

        public string PropertyName { get; set; }
    }
}