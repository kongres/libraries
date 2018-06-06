namespace Kongrevsky.Utilities.Web
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Authorization;

    public static class ActionDescriptorUtils
    {
        public static bool AttributeExists<T>(this ActionDescriptor actionDescriptor)
        {
            var filterDescriptors = actionDescriptor.FilterDescriptors.Where(x => x.Filter.GetType() == typeof(T));
            return filterDescriptors.Any();
        }

        public static IEnumerable<T> GetAttributes<T>(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor.FilterDescriptors?.Where(x => x.Filter.GetType() == typeof(T)).Select(x => x.Filter).OfType<T>();
        }

        public static bool AllowAnonymousAttributeExists(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor.AttributeExists<AllowAnonymousFilter>();
        }
    }
}