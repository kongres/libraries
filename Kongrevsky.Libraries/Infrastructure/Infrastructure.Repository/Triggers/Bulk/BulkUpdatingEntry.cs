namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public interface IBulkUpdatingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        List<BulkUpdatingEntity<TEntity>> Entities { get; set; }
        TDbContext Context { get; set; }

    }

    public class BulkUpdatingEntry<TEntity, TDbContext> : IBulkUpdatingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public BulkUpdatingEntry(List<BulkUpdatingEntity<TEntity>> entities, TDbContext context)
        {
            Entities = entities;
            Context = context;
        }

        public List<BulkUpdatingEntity<TEntity>> Entities { get; set; }

        public TDbContext Context { get; set; }
    }

    public class BulkUpdatingEntity<TEntity>
    {
        public BulkUpdatingEntity(TEntity original, TEntity entity)
        {
            Original = original;
            Entity = entity;
        }

        public TEntity Original { get; set; }

        public TEntity Entity { get; set; }

    }
}