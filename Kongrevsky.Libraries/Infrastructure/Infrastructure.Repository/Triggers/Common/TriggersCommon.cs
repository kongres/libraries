namespace Kongrevsky.Infrastructure.Repository.Triggers.Common
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Kongrevsky.Utilities.Enumerable;
    using Kongrevsky.Utilities.Expression;

    #endregion

    public static class TriggersCommon<TDbContext> where TDbContext : DbContext
    {
        private static readonly List<Action<ICommitExecutingContext<TDbContext>>> commitExecuting = new List<Action<ICommitExecutingContext<TDbContext>>>();

        private static readonly List<Action<ICommitExecutedContext<TDbContext>>> commitExecuted = new List<Action<ICommitExecutedContext<TDbContext>>>();

        public static event Action<ICommitExecutingContext<TDbContext>> CommitExecuting
        {
            add => commitExecuting.Add(value);
            remove => commitExecuting.Remove(value);
        }

        public static event Action<ICommitExecutedContext<TDbContext>> CommitExecuted
        {
            add => commitExecuted.Add(value);
            remove => commitExecuted.Remove(value);
        }

        public static ICommitExecutingContext<TDbContext> RaiseCommitExecuting(TDbContext dbContext)
        {

            if (commitExecuted.Any())
                commitExecuting.ForEach(x => x.Invoke(new CommitExecutingContext<TDbContext>(dbContext)));
            return new CommitExecutingContext<TDbContext>(dbContext);
        }

        public static void RaiseCommitExecuted(TDbContext dbContext, ICommitExecutingContext<TDbContext> commitExecutingContext)
        {
            if (!commitExecuted.Any())
                return;
            var context = new CommitExecutedContext<TDbContext>(dbContext, commitExecutingContext);
            commitExecuted.ForEach(x => x.Invoke(context));
        }
    }
}