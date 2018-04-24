namespace Kongrevsky.Infrastructure.Repository.Triggers
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public interface IBulkUpdatedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        List<TEntity> Entities { get; set; }
        TDbContext Context { get; set; }

    }

    public class BulkUpdatedEntry<TEntity, TDbContext> : IBulkUpdatedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public BulkUpdatedEntry(List<TEntity> entities, TDbContext context)
        {
            Entities = entities;
            Context = context;
        }

        public List<TEntity> Entities { get; set; }

        public TDbContext Context { get; set; }
    }

}