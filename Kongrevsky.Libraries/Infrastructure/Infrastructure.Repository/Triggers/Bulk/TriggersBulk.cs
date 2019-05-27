namespace Kongrevsky.Infrastructure.Repository.Triggers.Bulk
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
    using LinqKit;

    #endregion

    public static class TriggersBulk<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
    {
        private static readonly List<Action<IInsertingEntry<TEntity, TDbContext>>> inserting = new List<Action<IInsertingEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IBulkInsertingEntry<TEntity, TDbContext>>> bulkInserting = new List<Action<IBulkInsertingEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IUpdatingEntry<TEntity, TDbContext>>> updating = new List<Action<IUpdatingEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IBulkUpdatingEntry<TEntity, TDbContext>>> bulkUpdating = new List<Action<IBulkUpdatingEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IDeletingEntry<TEntity, TDbContext>>> deleting = new List<Action<IDeletingEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IBulkDeletingEntry<TEntity, TDbContext>>> bulkDeleting = new List<Action<IBulkDeletingEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IEnumerable<TEntity>, TDbContext>> insertFailed = new List<Action<IEnumerable<TEntity>, TDbContext>>();

        private static readonly List<Action<IEnumerable<TEntity>, TDbContext>> updateFailed = new List<Action<IEnumerable<TEntity>, TDbContext>>();

        private static readonly List<Action<IEnumerable<TEntity>, TDbContext>> deleteFailed = new List<Action<IEnumerable<TEntity>, TDbContext>>();

        private static readonly List<Action<IInsertedEntry<TEntity, TDbContext>>> inserted = new List<Action<IInsertedEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IBulkInsertedEntry<TEntity, TDbContext>>> bulkInserted = new List<Action<IBulkInsertedEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IUpdatedEntry<TEntity, TDbContext>>> updated = new List<Action<IUpdatedEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IBulkUpdatedEntry<TEntity, TDbContext>>> bulkUpdated = new List<Action<IBulkUpdatedEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IDeletedEntry<TEntity, TDbContext>>> deleted = new List<Action<IDeletedEntry<TEntity, TDbContext>>>();

        private static readonly List<Action<IBulkDeletedEntry<TEntity, TDbContext>>> bulkDeleted = new List<Action<IBulkDeletedEntry<TEntity, TDbContext>>>();

        public static event Action<IInsertingEntry<TEntity, TDbContext>> Inserting
        {
            add => inserting.Add(value);
            remove => inserting.Remove(value);
        }

        public static event Action<IBulkInsertingEntry<TEntity, TDbContext>> BulkInserting
        {
            add => bulkInserting.Add(value);
            remove => bulkInserting.Remove(value);
        }

        public static event Action<IUpdatingEntry<TEntity, TDbContext>> Updating
        {
            add => updating.Add(value);
            remove => updating.Remove(value);
        }

        public static event Action<IBulkUpdatingEntry<TEntity, TDbContext>> BulkUpdating
        {
            add => bulkUpdating.Add(value);
            remove => bulkUpdating.Remove(value);
        }

        public static event Action<IDeletingEntry<TEntity, TDbContext>> Deleting
        {
            add => deleting.Add(value);
            remove => deleting.Remove(value);
        }

        public static event Action<IBulkDeletingEntry<TEntity, TDbContext>> BulkDeleting
        {
            add => bulkDeleting.Add(value);
            remove => bulkDeleting.Remove(value);
        }

        public static event Action<IEnumerable<TEntity>, TDbContext> InsertFailed
        {
            add => insertFailed.Add(value);
            remove => insertFailed.Remove(value);
        }

        public static event Action<IEnumerable<TEntity>, TDbContext> UpdateFailed
        {
            add => updateFailed.Add(value);
            remove => updateFailed.Remove(value);
        }

        public static event Action<IEnumerable<TEntity>, TDbContext> DeleteFailed
        {
            add => deleteFailed.Add(value);
            remove => deleteFailed.Remove(value);
        }

        public static event Action<IInsertedEntry<TEntity, TDbContext>> Inserted
        {
            add => inserted.Add(value);
            remove => inserted.Remove(value);
        }

        public static event Action<IBulkInsertedEntry<TEntity, TDbContext>> BulkInserted
        {
            add => bulkInserted.Add(value);
            remove => bulkInserted.Remove(value);
        }

        public static event Action<IUpdatedEntry<TEntity, TDbContext>> Updated
        {
            add => updated.Add(value);
            remove => updated.Remove(value);
        }

        public static event Action<IBulkUpdatedEntry<TEntity, TDbContext>> BulkUpdated
        {
            add => bulkUpdated.Add(value);
            remove => bulkUpdated.Remove(value);
        }

        public static event Action<IDeletedEntry<TEntity, TDbContext>> Deleted
        {
            add => deleted.Add(value);
            remove => deleted.Remove(value);
        }

        public static event Action<IBulkDeletedEntry<TEntity, TDbContext>> BulkDeleted
        {
            add => bulkDeleted.Add(value);
            remove => bulkDeleted.Remove(value);
        }

        public static void RaiseInserting(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;
            foreach (var entity in list)
                inserting.ForEach(x => x.Invoke(new InsertingEntry<TEntity, TDbContext>(entity, dbContext)));
            bulkInserting.ForEach(x => x.Invoke(new BulkInsertingEntry<TEntity, TDbContext>(list, dbContext)));
        }

        public static void RaiseUpdating(IEnumerable<TEntity> entities, TDbContext dbContext, Expression<Func<TEntity, object>> identificator)
        {
            var funcIdentificator = identificator.Compile();
            var list = entities.ToList();
            if (!list.Any())
                return;

            var originals = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(500))
            {
                originals.AddRange(QueryableContains(dbContext.Set<TEntity>().AsNoTracking().AsExpandable(), chunk, identificator).ToList());
            }

            var eventParams = new List<BulkUpdatingEntity<TEntity>>();
            foreach (var entity in list)
            {
                var id = funcIdentificator(entity);
                var original = originals.FirstOrDefault(x => Equals(funcIdentificator(x), id));
                if (original == null)
                    continue;
                updating.ForEach(x => x.Invoke(new UpdatingEntry<TEntity, TDbContext>(original, entity, dbContext)));
                eventParams.Add(new BulkUpdatingEntity<TEntity>(original, entity));
            }

            bulkUpdating.ForEach(x => x.Invoke(new BulkUpdatingEntry<TEntity, TDbContext>(eventParams, dbContext)));
        }

        public static void RaiseDeleting(IEnumerable<TEntity> entities, TDbContext dbContext, Expression<Func<TEntity, object>> identificator)
        {
            var funcIdentificator = identificator.Compile();
            var list = entities.ToList();
            if (!list.Any())
                return;

            var deletingEntities = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(500))
            {
                deletingEntities.AddRange(QueryableContains(dbContext.Set<TEntity>().AsNoTracking().AsExpandable(), chunk, identificator).ToList());
            }

            foreach (var entity in list)
            {
                var id = funcIdentificator(entity);
                var original = deletingEntities.FirstOrDefault(x => Equals(funcIdentificator(x), id));
                if (original == null)
                    continue;
                deleting.ForEach(x => x.Invoke(new DeletingEntry<TEntity, TDbContext>(original, dbContext)));
            }

            bulkDeleting.ForEach(x => x.Invoke(new BulkDeletingEntry<TEntity, TDbContext>(deletingEntities, dbContext)));
        }

        public static void RaiseDeleting(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.Where(x => x != null).ToList();
            if (!list.Any())
                return;

            foreach (var entity in list)
            {
                deleting.ForEach(x => x.Invoke(new DeletingEntry<TEntity, TDbContext>(entity, dbContext)));
            }

            bulkDeleting.ForEach(x => x.Invoke(new BulkDeletingEntry<TEntity, TDbContext>(list, dbContext)));
        }

        public static void RaiseInsertFailed(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            insertFailed.ForEach(x => x.Invoke(entities, dbContext));
        }

        public static void RaiseUpdateFailed(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            updateFailed.ForEach(x => x.Invoke(entities, dbContext));
        }

        public static void RaiseDeleteFailed(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            deleteFailed.ForEach(x => x.Invoke(entities, dbContext));
        }

        public static void RaiseInserted(IEnumerable<TEntity> entities, TDbContext dbContext, Expression<Func<TEntity, object>> identificator)
        {
            var funcIdentificator = identificator.Compile();
            var list = entities.ToList();
            if (!list.Any())
                return;

            var insertedEntities = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(500))
            {
                insertedEntities.AddRange(QueryableContains(dbContext.Set<TEntity>().AsNoTracking().AsExpandable(), chunk, identificator).ToList());
            }

            foreach (var entity in list)
            {
                var id = funcIdentificator(entity);
                var original = insertedEntities.FirstOrDefault(x => Equals(funcIdentificator(x), id));
                if (original == null)
                    continue;
                inserted.ForEach(x => x.Invoke(new InsertedEntry<TEntity, TDbContext>(original, dbContext)));
            }

            bulkInserted.ForEach(x => x.Invoke(new BulkInsertedEntry<TEntity, TDbContext>(insertedEntities, dbContext)));
        }

        public static void RaiseUpdated(IEnumerable<TEntity> entities, TDbContext dbContext, Expression<Func<TEntity, object>> identificator)
        {
            var funcIdentificator = identificator.Compile();
            var list = entities.ToList();
            if (!list.Any())
                return;

            var updatedEntities = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(500))
            {
                updatedEntities.AddRange(QueryableContains(dbContext.Set<TEntity>().AsNoTracking().AsExpandable(), chunk, identificator).ToList());
            }

            foreach (var entity in list)
            {
                var id = funcIdentificator(entity);
                var original = updatedEntities.FirstOrDefault(x => Equals(funcIdentificator(x), id));
                if (original == null)
                    continue;
                updated.ForEach(x => x.Invoke(new UpdatedEntry<TEntity, TDbContext>(original, dbContext)));
            }

            bulkUpdated.ForEach(x => x.Invoke(new BulkUpdatedEntry<TEntity, TDbContext>(updatedEntities, dbContext)));
        }

        public static void RaiseDeleted(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;
            foreach (var entity in list)
                deleted.ForEach(x => x.Invoke(new DeletedEntry<TEntity, TDbContext>(entity, dbContext)));
            bulkDeleted.ForEach(x => x.Invoke(new BulkDeletedEntry<TEntity, TDbContext>(list, dbContext)));
        }

        private static IQueryable<TEntity> QueryableContains(IQueryable<TEntity> queryable, IEnumerable<TEntity> list, Expression<Func<TEntity, object>> identificator)
        {
            var idType = identificator.Body.Type;
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            var selectedList = list.Select(identificator.Compile()).ToList().ChangeType(idType);
            var containsMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().FirstOrDefault(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Count() == 2)?.MakeGenericMethod(idType);

            Expression expression;

            if (containsMethod == null)
                expression = Expression.Constant(true);
            else
                expression = Expression.Call(null, containsMethod, Expression.Constant(selectedList), Expression.Invoke(identificator.ToExpression(), parameter));

            var compExpr = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
            return queryable.Where(compExpr);
        }
    }
}