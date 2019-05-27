namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public interface IBulkInsertedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        List<TEntity> Entities { get; set; }
        TDbContext Context { get; set; }

    }

    public class BulkInsertedEntry<TEntity, TDbContext> : IBulkInsertedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public BulkInsertedEntry(List<TEntity> entities, TDbContext context)
        {
            Entities = entities;
            Context = context;
        }

        public List<TEntity> Entities { get; set; }

        public TDbContext Context { get; set; }
    }

}