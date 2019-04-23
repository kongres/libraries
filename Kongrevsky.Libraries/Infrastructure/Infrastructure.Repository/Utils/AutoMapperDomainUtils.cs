namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.Reflection;

    #endregion

    public static class AutoMapperDomainUtils
    {
        public static IMapper GetMapper(Action<IMapperConfigurationExpression> configure)
        {
            return GetConfigurationProvider(configure).CreateMapper();
        }

        public static IConfigurationProvider GetConfigurationProvider(Action<IMapperConfigurationExpression> configure)
        {
            var config = new MapperConfiguration(conf => { configure?.Invoke(conf); });
            return config;
        }

        public static IMappingExpression<TSource, TDestination> LoadProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression, RepositoryPagingModel<TDestination> filter)
        {
            var destType = typeof(TDestination);

            if (!string.IsNullOrEmpty(filter.Distinct))
            {

                filter.LoadProperties = new List<string>() { filter.Distinct.Split(new[] { '.' }, 2)[0] };
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

            var orderProperties = filter.OrderProperty?.Split(new[] { ',', ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            if (orderProperties.Any())
            {
                foreach (var property in orderProperties)
                {
                    var split = property.Split(new[] { '.' }, 2);
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
                var propertyValuePairs = QueryableUtils.ToPropertyValuePairs(filter.Filters, destType);
                properties.AddRange(propertyValuePairs.SelectMany(x => x.Select(c => c.Property.Name)));
            }

            var propertyInfos = infos.Where(x => !properties.Contains(x.Name, new GenericCompare<string>(s => s.ToLowerInvariant())) && (!x.GetCustomAttributes(typeof(LoadAlways), true).Any() || x.GetCustomAttributes(typeof(NotLoadByDefault), true).Any())).ToList();

            foreach (var property in propertyInfos)
                expression.ForMember(property.Name, opt => opt.Ignore());

            return expression;
        }
    }
}