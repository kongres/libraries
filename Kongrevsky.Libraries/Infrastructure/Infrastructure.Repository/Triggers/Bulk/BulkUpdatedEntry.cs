namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public interface IBulkDeletedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        List<TEntity> Entities { get; set; }
        TDbContext Context { get; set; }

    }

    public class BulkDeletedEntry<TEntity, TDbContext> : IBulkDeletedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public BulkDeletedEntry(List<TEntity> entities, TDbContext context)
        {
            Entities = entities;
            Context = context;
        }

        public List<TEntity> Entities { get; set; }

        public TDbContext Context { get; set; }
    }

}