namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    #endregion

    public class RepositoryPagingModelUtils
    {
        public static object ExcludeIgnoredPropertiesPagingModel<TItem>(RepositoryPagingModel<TItem> filter, List<string> loadProperties, Type itemType)
                where TItem : class
        {
            return JsonConvert.SerializeObject(filter, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new PagingContractResolver(loadProperties, itemType) });
        }
    }

    public class PagingContractResolver : DefaultContractResolver
    {
        private readonly Type _itemType;

        private readonly List<string> _loadProperties;

        public PagingContractResolver(List<string> loadProperties, Type itemType)
        {
            this._loadProperties = loadProperties.Select(x => x.Split(new[] { '.' }, 2)[0]).Where(x => !string.IsNullOrEmpty(x)).ToList();
            this._itemType = itemType;
            NamingStrategy = new CamelCaseNamingStrategy()
                             {
                                     ProcessDictionaryKeys = true,
                                     OverrideSpecifiedNames = true
                             };
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (type != this._itemType)
                return properties;

            properties = properties.Where(p => this._loadProperties.Any(x => x.Equals(p.PropertyName, StringComparison.InvariantCultureIgnoreCase))).ToList();

            return properties;
        }
    }
}