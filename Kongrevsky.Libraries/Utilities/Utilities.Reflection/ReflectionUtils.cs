namespace Utilities.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ReflectionUtils
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentFullMethodName()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            var methodBase = sf.GetMethod();
            return methodBase.ReflectedType?.FullName + "." + methodBase.Name;
        }

        // Spec for Dictionary makes no such promise, and while
        // it should be okay in this case, play it safe.
        public static List<PropertyInfo> GetPropertiesByType<T>(this Type type)
        {
            var list = new List<PropertyInfo>();
            foreach (var prop in type.GetProperties())
            {
                var valType = prop.PropertyType;
                if (valType == typeof(T))
                    list.Add(prop);
            }
            if (list.Count == 0)
                return EmptyPropsList; // Don't waste memory on lots of empty lists.
            list.TrimExcess();
            return list;
        }
        private static readonly List<PropertyInfo> EmptyPropsList = new List<PropertyInfo>();
    }
}