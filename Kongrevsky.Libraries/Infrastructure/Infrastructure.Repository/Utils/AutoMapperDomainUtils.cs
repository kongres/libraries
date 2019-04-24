namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Z.EntityFramework.Plus;
    #endregion

    public static class AutoMapperDomainUtils
    {
        public static Action<IMapperConfigurationExpression> CommonConfiguration { get; set; } = config => { };

        public static IMapper GetMapper(Action<IMapperConfigurationExpression> configure)
        {
            return GetConfigurationProvider(configure).CreateMapper();
        }

        public static IConfigurationProvider GetConfigurationProvider(Action<IMapperConfigurationExpression> configure)
        {
            return new MapperConfiguration(conf =>
                                           {
                                               CommonConfiguration?.Invoke(conf);
                                               configure?.Invoke(conf);
                                           });
        }

        public static IMappingExpression<TSource, TDestination> LoadProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression, RepositoryPagingModel<TDestination> filter, DbContext context = null) 
                where TSource : class
        {
            var destType = typeof(TDestination);
            var infos = destType.GetProperties();

            if (!string.IsNullOrEmpty(filter.Distinct))
                filter.LoadProperties = new List<string>() { filter.Distinct.Split(new[] { '.' }, 2)[0] };

            var properties = (filter.LoadProperties ?? (filter.LoadProperties = new List<string>())).ToList();

            if (!properties.Any())
            {
                var propertyInfoIgnore = infos.Where(x => x.GetCustomAttributes(typeof(NotLoadByDefault), true).Any()).ToList();

                foreach (var property in propertyInfoIgnore)
                    expression.ForMember(property.Name, opt => opt.Ignore());
                return expression;
            }

            var orderProperties = filter.OrderProperty?.Split(new[] { ',', ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            if (string.IsNullOrEmpty(filter.Distinct) && context != null)
                orderProperties.AddRange(context.GetKeyNames<TSource>());

            if (orderProperties.Any())
            {
                foreach (var property in orderProperties)
                {
                    var split = property.Split(new[] { '.' }, 2)[0];
                    if(string.IsNullOrEmpty(split))
                        continue;

                    var orderProperty = infos.FirstOrDefault(x => string.Equals(x.Name, split, StringComparison.InvariantCultureIgnoreCase));

                    if (orderProperty != null && !properties.Contains(orderProperty.Name, StringComparer.OrdinalIgnoreCase))
                        properties.Add(orderProperty.Name);
                }
            }

            var orderPropertyDefault = infos.FirstOrDefault(x => x.GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true).Any());
            if (orderPropertyDefault != null && !properties.Contains(orderPropertyDefault.Name, StringComparer.OrdinalIgnoreCase))
                properties.Add(orderPropertyDefault.Name);

            if (filter.Filters?.Any() ?? false)
            {
                var propertyValuePairs = QueryableUtils.ToPropertyValuePairs(filter.Filters, destType);
                properties.AddRange(propertyValuePairs.SelectMany(x => x.Select(c => c.Property.Name)));
            }

            var propertyInfos = infos.Where(x => !properties.Contains(x.Name, StringComparer.OrdinalIgnoreCase) && (!x.GetCustomAttributes(typeof(LoadAlways), true).Any() || x.GetCustomAttributes(typeof(NotLoadByDefault), true).Any())).ToList();

            foreach (var property in propertyInfos)
                expression.ForMember(property.Name, opt => opt.Ignore());

            return expression;
        }
    }
}