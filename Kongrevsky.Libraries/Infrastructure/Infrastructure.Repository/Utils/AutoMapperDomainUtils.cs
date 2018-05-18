namespace Kongrevsky.Infrastructure.Repository.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.Reflection;

    public static class AutoMapperDomainUtils
    {
        public static IMapper GetMapper(Action<IMapperConfigurationExpression> configure)
        {
            return GetConfigurationProvider(configure).CreateMapper();
        }

        public static IConfigurationProvider GetConfigurationProvider(Action<IMapperConfigurationExpression> configure)
        {
            var config = new MapperConfiguration(conf =>
                                                 {
                                                     configure?.Invoke(conf);
                                                 });
            return config;
        }

        public static bool HasAccessWithoutPermission<Source, Target, Obj>(this IMemberConfigurationExpression<Source, Target, Obj> expression)
        {
            var attrs = expression.DestinationMember.CustomAttributes;
            return attrs.Any(x => x.AttributeType == typeof(AccessWithoutPermissionAttribute));
        }

        public static IMappingExpression<TSource, TDestination> LoadProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression, PagingModel<TDestination> filter)
        {
            var destType = typeof(TDestination);

            if (!string.IsNullOrEmpty(filter.Distinct))
            {
                var distinctProperty = destType.GetPropertyByName(filter.Distinct);
                if (distinctProperty != null && distinctProperty.PropertyType.IsSimple())
                    filter.LoadProperties = new List<string>() { distinctProperty.Name };
            }

            var properties = (filter.LoadProperties ?? (filter.LoadProperties = new List<string>())).ToList();

            var infos = destType.GetProperties();

            if (!properties.Any())
            {
                var propertyInfoIgnore = infos.Where(x => x.GetCustomAttributes(typeof(NotLoadByDefault), true).Any()).ToList();

                foreach (var property in propertyInfoIgnore)
                    expression.ForMember(property.Name, opt => opt.Ignore());
                return expression;
            }

            properties.Add("Id");

            var orderProperties = filter.OrderProperty?.Split(new[] { ',', ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            if (orderProperties.Any())
            {
                foreach (var property in orderProperties)
                {
                    var split = property.Split(new char[] { '.' }, 2);
                    var orderProperty = infos.FirstOrDefault(x => !string.IsNullOrEmpty(split[0]) && string.Equals(x.Name, split[0], StringComparison.InvariantCultureIgnoreCase));

                    if (orderProperty != null && !properties.Contains(orderProperty.Name, new GenericCompare<string>(x => x.ToLowerInvariant())))
                        properties.Add(orderProperty.Name);
                }
            }
            var orderPropertyDefault = infos.FirstOrDefault(x => x.GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true).Any());
            if (orderPropertyDefault != null && !properties.Contains(orderPropertyDefault.Name, new GenericCompare<string>(x => x.ToLowerInvariant())))
                properties.Add(orderPropertyDefault.Name);


            if (filter.Filters?.Any() ?? false)
            {
                var propertyValuePairs = QueryableUtils.ToPropertyValuePairs(filter.Filters,destType);
                properties.AddRange(propertyValuePairs.SelectMany(x => x.Select(c => c.Property.Name)));
            }

            var propertyInfos = infos.Where(x => !properties.Contains(x.Name, new GenericCompare<string>(s => s.ToLowerInvariant())) && (!x.GetCustomAttributes(typeof(LoadAlways), true).Any() || x.GetCustomAttributes(typeof(NotLoadByDefault), true).Any())).ToList();

            foreach (var property in propertyInfos)
                expression.ForMember(property.Name, opt => opt.Ignore());

            return expression;
        }
    }
}