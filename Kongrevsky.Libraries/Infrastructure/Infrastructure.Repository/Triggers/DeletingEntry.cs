namespace Kongrevsky.Infrastructure.Repository.Triggers {
    using System.Data.Entity;

    public interface IDeletingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        TEntity Entity { get; set; }

        TDbContext Context { get; set; }
    }

    public class DeletingEntry<TEntity, TDbContext> : IDeletingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        public DeletingEntry(TEntity entity, TDbContext context)
        {
            Entity = entity;
            Context = context;
        }

        public TEntity Entity { get; set; }

        public TDbContext Context { get; set; }
    }
}