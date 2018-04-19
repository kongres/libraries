namespace Infrastructure.Repository.Triggers {
    using System.Data.Entity;

    public interface IUpdatedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        TEntity Entity { get; set; }

        TDbContext Context { get; set; }
    }

    public class UpdatedEntry<TEntity, TDbContext> : IUpdatedEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        public UpdatedEntry(TEntity entity, TDbContext context)
        {
            Entity = entity;
            Context = context;
        }

        public TEntity Entity { get; set; }

        public TDbContext Context { get; set; }
    }
}