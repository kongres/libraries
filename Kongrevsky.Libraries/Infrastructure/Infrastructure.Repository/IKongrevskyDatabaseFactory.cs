namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.Linq;

    #endregion

    public interface IKongrevskyDatabaseFactory<T> : IDisposable where T : KongrevskyDbContext
    {
        T Get();

        IQueryable<TSet> GetDbSet<TSet>() where TSet : class;
    }
}