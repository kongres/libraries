namespace Infrastructure.Repository.Triggers
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public interface IBulkInsertingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        List<TEntity> Entities { get; set; }
        TDbContext Context { get; set; }

    }

    public class BulkInsertingEntry<TEntity, TDbContext> : IBulkInsertingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public BulkInsertingEntry(List<TEntity> entities, TDbContext context)
        {
            Entities = entities;
            Context = context;
        }

        public List<TEntity> Entities { get; set; }

        public TDbContext Context { get; set; }
    }

}