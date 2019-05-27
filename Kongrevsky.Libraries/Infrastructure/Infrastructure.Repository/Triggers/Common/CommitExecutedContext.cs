namespace Kongrevsky.Infrastructure.Repository.Triggers.Common
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    #endregion

    public interface ICommitExecutedContext<TDbContext> where TDbContext : DbContext
    {
        List<object> InsertedEntities { get; set; }

        List<object> UpdatedEntities { get; set; }

        List<object> DeletedEntities { get; set; }

        TDbContext Context { get; set; }
        CommitExecutedEntities<T> Entity<T>() where T : class;
    }

    public class CommitExecutedContext<TDbContext> : ICommitExecutedContext<TDbContext> where TDbContext : DbContext
    {
        public CommitExecutedContext(TDbContext context, ICommitExecutingContext<TDbContext> commitExecutingContext)
        {
            Context = context;
            var list = Context.ChangeTracker.Entries().ToList();
            InsertedEntities = commitExecutingContext.InsertingEntities.ToList();
            UpdatedEntities = commitExecutingContext.UpdatingEntities.Select(x => x.Entity).ToList();
            DeletedEntities = commitExecutingContext.DeletingEntities.ToList();
        }

        public List<object> InsertedEntities { get; set; }

        public List<object> UpdatedEntities { get; set; }

        public List<object> DeletedEntities { get; set; }

        public TDbContext Context { get; set; }


        public CommitExecutedEntities<T> Entity<T>() where T : class
        {
            return new CommitExecutedEntities<T>(InsertedEntities.OfType<T>().ToList(),
                                                              UpdatedEntities.OfType<T>().ToList(),
                                                              DeletedEntities.OfType<T>().ToList());
        }
    }

    public class CommitExecutedEntities<TEntity> where TEntity : class
    {
        public CommitExecutedEntities(List<TEntity> insertingEntities, List<TEntity> updatingEntities, List<TEntity> deletingEntities)
        {
            InsertingEntities = insertingEntities;
            UpdatingEntities = updatingEntities;
            DeletingEntities = deletingEntities;
        }

        public List<TEntity> InsertingEntities { get; }

        public List<TEntity> UpdatingEntities { get; }

        public List<TEntity> DeletingEntities { get; }
    }
}