namespace Infrastructure.Repository
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public interface IDatabaseFactory<T> : IDisposable where T : DbContext
    {
        T Get();

        IQueryable<TSet> GetDbSet<TSet>() where TSet : class;
    }
}