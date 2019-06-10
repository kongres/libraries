namespace Kongrevsky.Infrastructure.Repository.Triggers.Common
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    #endregion

    public interface ICommitExecutedContext<TDbContext> where TDbContext : DbContext
    {
        List<object> InsertedEntities { get; }

        List<object> UpdatedEntities { get; }

        List<object> DeletedEntities { get; }

        TDbContext Context { get; }

        CommitExecutedEntities<T> Entity<T>() where T : class;

        /// <summary>
        /// Has any changed entities
        /// </summary>
        /// <returns></returns>
        bool Any();
    }

    public class CommitExecutedContext<TDbContext> : ICommitExecutedContext<TDbContext> where TDbContext : DbContext
    {
        public CommitExecutedContext(TDbContext context, ICommitExecutingContext<TDbContext> commitExecutingContext)
        {
            Context = context;
            InsertedEntities = commitExecutingContext.InsertingEntities.ToList();
            UpdatedEntities = commitExecutingContext.UpdatingEntities.Select(x => x.Entity).ToList();
            DeletedEntities = commitExecutingContext.DeletingEntities.ToList();
        }

        public List<object> InsertedEntities { get; }

        public List<object> UpdatedEntities { get; }

        public List<object> DeletedEntities { get; }

        public TDbContext Context { get; }

        public CommitExecutedEntities<T> Entity<T>() where T : class
        {
            return new CommitExecutedEntities<T>(InsertedEntities.OfType<T>().ToList(),
                                                 UpdatedEntities.OfType<T>().ToList(),
                                                 DeletedEntities.OfType<T>().ToList());
        }

        public bool Any()
        {
            return InsertedEntities.Any() || UpdatedEntities.Any() || DeletedEntities.Any();
        }
    }

    public class CommitExecutedEntities<TEntity> where TEntity : class
    {
        public CommitExecutedEntities(List<TEntity> insertedEntities, List<TEntity> updatedEntities, List<TEntity> deletedEntities)
        {
            InsertedEntities = insertedEntities;
            UpdatedEntities = updatedEntities;
            DeletedEntities = deletedEntities;
        }

        public List<TEntity> InsertedEntities { get; }

        public List<TEntity> UpdatedEntities { get; }

        public List<TEntity> DeletedEntities { get; }

        /// <summary>
        /// Has any changed entities
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return InsertedEntities.Any() || UpdatedEntities.Any() || DeletedEntities.Any();
        }
    }
}