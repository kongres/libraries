namespace Infrastructure.Repository.Triggers {
    using System.Data.Entity;

    public interface IInsertingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        TEntity Entity { get; set; }

        TDbContext Context { get; set; }
    }

    public class InsertingEntry<TEntity, TDbContext> : IInsertingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        public InsertingEntry(TEntity entity, TDbContext context)
        {
            Entity = entity;
            Context = context;
        }

        public TEntity Entity { get; set; }

        public TDbContext Context { get; set; }
    }
}