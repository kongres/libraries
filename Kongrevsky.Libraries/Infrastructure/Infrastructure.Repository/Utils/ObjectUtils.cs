namespace Infrastructure.Repository.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kongrevsky.Utilities.Reflection;

    internal class ObjectUtils
    {
        public static void FixDates(object entity)
        {
            if (entity == null)
                return;

            var eType = entity.GetType();
            var rules = (List<PropertyInfo>)PropsCache[eType];
            if (rules == null)
                lock (PropsCache)
                    PropsCache[eType] = rules = eType.GetPropertiesByType<DateTime>().Union(eType.GetPropertiesByType<DateTime?>()).ToList(); // Don't bother double-checking. Over-write is safe.
            foreach (var rule in rules)
            {
                var curVal = rule.GetValue(entity);
                if (curVal != null)
                    rule.SetValue(entity, DateTime.SpecifyKind((DateTime)curVal, DateTimeKind.Utc));
            }
        }
        private static readonly Hashtable PropsCache = new Hashtable(); // Spec promises safe for single-reader, multiple writer.

    }
}