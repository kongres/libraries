namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk {
    using System.Data.Entity;

    public interface IDeletedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        TEntity Entity { get; set; }

        TDbContext Context { get; set; }
    }

    public class DeletedEntry<TEntity, TDbContext> : IDeletedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        public DeletedEntry(TEntity entity, TDbContext context)
        {
            Entity = entity;
            Context = context;
        }

        public TEntity Entity { get; set; }

        public TDbContext Context { get; set; }
    }
}