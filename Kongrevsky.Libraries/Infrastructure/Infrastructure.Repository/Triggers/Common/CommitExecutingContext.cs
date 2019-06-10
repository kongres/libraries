namespace Kongrevsky.Infrastructure.Repository.Triggers.Common
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using EntityFramework.TypedOriginalValues;

    #endregion

    public interface ICommitExecutingContext<TDbContext> where TDbContext : DbContext
    {
        List<object> InsertingEntities { get; }

        List<UpdatingEntity<object, TDbContext>> UpdatingEntities { get; }

        List<object> DeletingEntities { get; }

        TDbContext Context { get; }

        CommitExecutingEntities<T, TDbContext> Entity<T>() where T : class;

        /// <summary>
        ///     Has any changed entities
        /// </summary>
        /// <returns></returns>
        bool Any();
    }

    public class CommitExecutingContext<TDbContext> : ICommitExecutingContext<TDbContext> where TDbContext : DbContext
    {
        public CommitExecutingContext(TDbContext context)
        {
            Context = context;
            var list = Context.ChangeTracker.Entries().ToList();
            InsertingEntities = list.Where(x => x.State == EntityState.Added).Select(x => x.Entity).ToList();
            UpdatingEntities = list.Where(x => x.State == EntityState.Modified).Select(x => x.Entity).Select(x => new UpdatingEntity<object, TDbContext>(x, Context)).ToList();
            DeletingEntities = list.Where(x => x.State == EntityState.Deleted).Select(x => x.Entity).ToList();
        }

        public List<object> InsertingEntities { get; }

        public List<UpdatingEntity<object, TDbContext>> UpdatingEntities { get; }

        public List<object> DeletingEntities { get; }

        public TDbContext Context { get; }

        public CommitExecutingEntities<T, TDbContext> Entity<T>() where T : class
        {
            return new CommitExecutingEntities<T, TDbContext>(InsertingEntities.OfType<T>().ToList(),
                                                              UpdatingEntities.Where(x => x.Entity.GetType() == typeof(T)).Select(x => new UpdatingEntity<T, TDbContext>((T)x.Entity, Context)).ToList(),
                                                              DeletingEntities.OfType<T>().ToList());
        }

        public bool Any()
        {
            return InsertingEntities.Any() || UpdatingEntities.Any() || DeletingEntities.Any();
        }
    }

    public class CommitExecutingEntities<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public CommitExecutingEntities(List<TEntity> insertingEntities, List<UpdatingEntity<TEntity, TDbContext>> updatingEntities, List<TEntity> deletingEntities)
        {
            InsertingEntities = insertingEntities;
            UpdatingEntities = updatingEntities;
            DeletingEntities = deletingEntities;
        }

        public List<TEntity> InsertingEntities { get; }

        public List<UpdatingEntity<TEntity, TDbContext>> UpdatingEntities { get; }

        public List<TEntity> DeletingEntities { get; }

        /// <summary>
        ///     Has any changed entities
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return InsertingEntities.Any() || UpdatingEntities.Any() || DeletingEntities.Any();
        }
    }

    public class UpdatingEntity<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        public UpdatingEntity(TEntity entity, TDbContext context)
        {
            this.context = context;
            this.original = new Lazy<TEntity>(() => this.context.GetOriginal(Entity));
            Entity = entity;
        }

        private TDbContext context { get; }

        private Lazy<TEntity> original { get; }

        public TEntity Original => original.Value;

        public TEntity Entity { get; }
    }
}