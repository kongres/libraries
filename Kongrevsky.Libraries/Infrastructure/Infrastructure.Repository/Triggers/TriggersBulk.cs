﻿namespace Infrastructure.Repository.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using CoContra;
    using Utilities.Enumerable;

    public static class TriggersBulk<TEntity, TDbContext> where TEntity : BaseEntity where TDbContext : DbContext
    {
        private static readonly CovariantAction<IInsertingEntry<TEntity, TDbContext>> inserting = new CovariantAction<IInsertingEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IBulkInsertingEntry<TEntity, TDbContext>> bulkInserting = new CovariantAction<IBulkInsertingEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IUpdatingEntry<TEntity, TDbContext>> updating = new CovariantAction<IUpdatingEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IBulkUpdatingEntry<TEntity, TDbContext>> bulkUpdating = new CovariantAction<IBulkUpdatingEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IDeletingEntry<TEntity, TDbContext>> deleting = new CovariantAction<IDeletingEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IBulkDeletingEntry<TEntity, TDbContext>> bulkDeleting = new CovariantAction<IBulkDeletingEntry<TEntity, TDbContext>>();

        private static readonly CovariantAction<IEnumerable<TEntity>, TDbContext> insertFailed = new CovariantAction<IEnumerable<TEntity>, TDbContext>();
        private static readonly CovariantAction<IEnumerable<TEntity>, TDbContext> updateFailed = new CovariantAction<IEnumerable<TEntity>, TDbContext>();
        private static readonly CovariantAction<IEnumerable<TEntity>, TDbContext> deleteFailed = new CovariantAction<IEnumerable<TEntity>, TDbContext>();

        private static readonly CovariantAction<IInsertedEntry<TEntity, TDbContext>> inserted = new CovariantAction<IInsertedEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IBulkInsertedEntry<TEntity, TDbContext>> bulkInserted = new CovariantAction<IBulkInsertedEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IUpdatedEntry<TEntity, TDbContext>> updated = new CovariantAction<IUpdatedEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IBulkUpdatedEntry<TEntity, TDbContext>> bulkUpdated = new CovariantAction<IBulkUpdatedEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IDeletedEntry<TEntity, TDbContext>> deleted = new CovariantAction<IDeletedEntry<TEntity, TDbContext>>();
        private static readonly CovariantAction<IBulkDeletedEntry<TEntity, TDbContext>> bulkDeleted = new CovariantAction<IBulkDeletedEntry<TEntity, TDbContext>>();

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
                inserting.Invoke(new InsertingEntry<TEntity, TDbContext>(entity, dbContext));
            bulkInserting.Invoke(new BulkInsertingEntry<TEntity, TDbContext>(list, dbContext));
        }

        public static void RaiseUpdating(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;

            var originals = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(1000))
            {
                var keys = chunk.Select(x => x.Id).ToList();
                originals.AddRange(dbContext.Set<TEntity>().AsNoTracking().Where(x => keys.Contains(x.Id)).ToList());
            }

            var eventParams = new List<BulkUpdatingEntity<TEntity>>();
            foreach (var entity in list)
            {
                var original = originals.FirstOrDefault(x => x.Id == entity.Id);
                if (original == null)
                    continue;
                updating.Invoke(new UpdatingEntry<TEntity, TDbContext>(original, entity, dbContext));
                eventParams.Add(new BulkUpdatingEntity<TEntity>(original, entity));
            }
            bulkUpdating.Invoke(new BulkUpdatingEntry<TEntity, TDbContext>(eventParams, dbContext));
        }

        public static void RaiseDeleting(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;

            var deletingEntities = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(1000))
            {
                var keys = chunk.Select(x => x.Id).ToList();
                deletingEntities.AddRange(dbContext.Set<TEntity>().AsNoTracking().Where(x => keys.Contains(x.Id)).ToList());
            }

            foreach (var entity in list)
            {
                var original = deletingEntities.FirstOrDefault(x => x.Id == entity.Id);
                if (original == null)
                    continue;
                deleting.Invoke(new DeletingEntry<TEntity, TDbContext>(original, dbContext));
            }
            bulkDeleting.Invoke(new BulkDeletingEntry<TEntity, TDbContext>(deletingEntities, dbContext));
        }

        public static void RaiseInsertFailed(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            insertFailed.Invoke(entities, dbContext);
        }

        public static void RaiseUpdateFailed(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            updateFailed.Invoke(entities, dbContext);
        }

        public static void RaiseDeleteFailed(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            deleteFailed.Invoke(entities, dbContext);
        }

        public static void RaiseInserted(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;

            var insertedEntities = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(1000))
            {
                var keys = chunk.Select(x => x.Id).ToList();
                insertedEntities.AddRange(dbContext.Set<TEntity>().AsNoTracking().Where(x => keys.Contains(x.Id)).ToList());
            }

            foreach (var entity in list)
            {
                var original = insertedEntities.FirstOrDefault(x => x.Id == entity.Id);
                if (original == null)
                    continue;
                inserted.Invoke(new InsertedEntry<TEntity, TDbContext>(original, dbContext));
            }
            bulkInserted.Invoke(new BulkInsertedEntry<TEntity, TDbContext>(insertedEntities, dbContext));
        }

        public static void RaiseUpdated(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;

            var updatedEntities = new List<TEntity>();
            foreach (var chunk in list.ChunkBy(1000))
            {
                var keys = chunk.Select(x => x.Id).ToList();
                updatedEntities.AddRange(dbContext.Set<TEntity>().AsNoTracking().Where(x => keys.Contains(x.Id)).ToList());
            }

            foreach (var entity in list)
            {
                var original = updatedEntities.FirstOrDefault(x => x.Id == entity.Id);
                if (original == null)
                    continue;
                updated.Invoke(new UpdatedEntry<TEntity, TDbContext>(original, dbContext));
            }
            bulkUpdated.Invoke(new BulkUpdatedEntry<TEntity, TDbContext>(updatedEntities, dbContext));
        }

        public static void RaiseDeleted(IEnumerable<TEntity> entities, TDbContext dbContext)
        {
            var list = entities.ToList();
            if (!list.Any())
                return;
            foreach (var entity in list)
                deleted.Invoke(new DeletedEntry<TEntity, TDbContext>(entity, dbContext));
            bulkDeleted.Invoke(new BulkDeletedEntry<TEntity, TDbContext>(list, dbContext));
        }
    }
}