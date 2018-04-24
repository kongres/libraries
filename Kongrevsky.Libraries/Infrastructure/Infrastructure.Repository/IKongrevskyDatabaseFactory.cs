namespace Kongrevsky.Infrastructure.Repository
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public interface IKongrevskyDatabaseFactory<T> : IDisposable where T : KongrevskyDbContext
    {
        T Get();

        IQueryable<TSet> GetDbSet<TSet>() where TSet : class;
    }
}