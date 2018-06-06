namespace Kongrevsky.Infrastructure.Web.Extensions
{
    #region << Using >>

    using System.IO.Compression;
    using Kongrevsky.Infrastructure.Web.Providers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class DeflateResponseCompressionExtensions
    {
        public static IServiceCollection AddDeflateResponseCompression(this IServiceCollection services, CompressionLevel compressionLevel = CompressionLevel.Optimal, bool enableForHttps = true)
        {
            services.Configure<GzipCompressionProviderOptions>(options => { options.Level = compressionLevel; });
            services.AddResponseCompression(options =>
                                            {
                                                options.EnableForHttps = enableForHttps;
                                                options.Providers.Add(new DeflateCompressionProvider());
                                            });

            return services;
        }
    }
}