namespace Kongrevsky.Infrastructure.Repository.Infrastructure
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class RepositoryExtensions
    {
        public static void AddKongrevskyRepository<Db>(this IServiceCollection services, string connectionString) where Db : KongrevskyDbContext, new()
        {
            services.AddScoped<IKongrevskyDatabaseFactory<Db>, KongrevskyDatabaseFactory<Db>>();
            services.AddScoped<IKongrevskyUnitOfWork<Db>, KongrevskyUnitOfWork<Db>>();
            services.TryAddTransient(typeof(IKongrevskyRepository<,>), typeof(KongrevskyRepository<,>));
            services.AddScoped(_ => (Db)Activator.CreateInstance(typeof(Db), connectionString));
        }
    }
}