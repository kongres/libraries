namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk
{
    using System.Data.Entity;

    public interface IUpdatingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        TEntity Original { get; set; }

        TEntity Entity { get; set; }

        TDbContext Context { get; set; }
    }

    public class UpdatingEntry<TEntity, TDbContext> : IUpdatingEntry<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public UpdatingEntry(TEntity original, TEntity entity, TDbContext context)
        {
            Original = original;
            Entity = entity;
            Context = context;
        }

        public TEntity Original { get; set; }
        public TEntity Entity { get; set; }
        public TDbContext Context { get; set; }
    }
}