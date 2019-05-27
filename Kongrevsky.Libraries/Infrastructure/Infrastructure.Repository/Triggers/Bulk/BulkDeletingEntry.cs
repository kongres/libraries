namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public interface IBulkDeletingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        List<TEntity> Entities { get; set; }
        TDbContext Context { get; set; }

    }

    public class BulkDeletingEntry<TEntity, TDbContext> : IBulkDeletingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public BulkDeletingEntry(List<TEntity> entities, TDbContext context)
        {
            Entities = entities;
            Context = context;
        }

        public List<TEntity> Entities { get; set; }

        public TDbContext Context { get; set; }
    }

}