namespace Kongrevsky.Infrastructure.Web.Utils
{
    #region << Using >>

    using System;
    using System.Collections.Concurrent;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Web.Models;
    using Kongrevsky.Utilities.Enum;
    using Kongrevsky.Utilities.Object;

    #endregion

    public static class AutoMapperUiUtils
    {
        private static readonly ConcurrentDictionary<int, IMapper> serviceProvidersAuto = new ConcurrentDictionary<int, IMapper>();

        public static bool IsCacheEnabled { get; set; } = true;

        public static Action<IMapperConfigurationExpression> CommonConfiguration { get; set; } = expression => { };

        public static IMapper GetMapper(Action<IMapperConfigurationExpression> configure = null)
        {
            var hashCode = configure?.Method.GetHashCode() ?? 0;

            if (IsCacheEnabled && serviceProvidersAuto.ContainsKey(hashCode))
                return serviceProvidersAuto[hashCode];

            var config = new MapperConfiguration(conf =>
                                                 {
                                                     conf.CreateMap<Enum, EnumVm>()
                                                             .ForMember(x => x.Id, c => c.MapFrom(y => y.GetValue()))
                                                             .ForMember(x => x.Name, c => c.MapFrom(y => y.GetDisplayName()));
                                                     conf.CreateMap<Enum, string>().ConstructUsing(c => c.GetDisplayName());

                                                     CommonConfiguration?.Invoke(conf);
                                                     configure?.Invoke(conf);
                                                 });
            config.CompileMappings();
            var mapper = config.CreateMapper();

            return IsCacheEnabled ? serviceProvidersAuto.AddOrUpdate(hashCode, mapper, (i, m) => m) : mapper;
        }
    }
}