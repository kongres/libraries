namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.Repository.Triggers.Common;
    using Kongrevsky.Utilities.EF6;

    #endregion

    public class KongrevskyUnitOfWork<T> : IKongrevskyUnitOfWork<T> where T : KongrevskyDbContext
    {
        public KongrevskyUnitOfWork(IKongrevskyDatabaseFactory<T> kongrevskyDatabaseFactory)
        {
            _kongrevskyDatabaseFactory = kongrevskyDatabaseFactory;
        }

        private T Database => _database ?? (_database = _kongrevskyDatabaseFactory.Get());

        private IKongrevskyDatabaseFactory<T> _kongrevskyDatabaseFactory { get; }

        private T _database { get; set; }

        public static Action<IEnumerable<Tuple<object, object>>> AddedBeforeSaveChanges { get; set; }

        public static Action<IEnumerable<Tuple<object, object>>> RemovedBeforeSaveChanges { get; set; }

        public static Action<IEnumerable<Tuple<object, object>>> AddedAfterSaveChanges { get; set; }

        public static Action<IEnumerable<Tuple<object, object>>> RemovedAfterSaveChanges { get; set; }

        public void Commit(bool fireTriggers = true)
        {
            if (fireTriggers)
            {
                var addedRelationships = Database.GetAddedRelationships().ToList();
                var deletedRelationships = Database.GetDeletedRelationships().ToList();

                AddedBeforeSaveChanges?.Invoke(addedRelationships);
                RemovedBeforeSaveChanges?.Invoke(deletedRelationships);

                var commitExecutingContext = TriggersCommon<T>.RaiseCommitExecuting(Database);

                try
                {
                    Database.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    var exceptions = new List<Exception>();
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        exceptions.Add(new Exception($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}"));
                    throw new AggregateException("Validation failed for one or more entities. See InnerExceptions.", exceptions);
                }

                TriggersCommon<T>.RaiseCommitExecuted(Database, commitExecutingContext);

                AddedAfterSaveChanges?.Invoke(addedRelationships);
                RemovedAfterSaveChanges?.Invoke(deletedRelationships);
            }
            else
            {
                Database.TriggersEnabled = false;
                Database.SaveChanges();
                Database.TriggersEnabled = true;
            }
        }

        public async Task CommitAsync(bool fireTriggers = true)
        {
            if (fireTriggers)
            {
                var addedRelationships = Database.GetAddedRelationships().ToList();
                var deletedRelationships = Database.GetDeletedRelationships().ToList();

                AddedBeforeSaveChanges?.Invoke(addedRelationships);
                RemovedBeforeSaveChanges?.Invoke(deletedRelationships);

                var commitExecutingContext = TriggersCommon<T>.RaiseCommitExecuting(Database);

                try
                {
                    await Database.SaveChangesAsync(Task.Factory.CancellationToken);
                }
                catch (DbEntityValidationException dbEx)
                {
                    var exceptions = new List<Exception>();
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        exceptions.Add(new Exception($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}"));
                    throw new AggregateException("Validation failed for one or more entities. See InnerExceptions.", exceptions);
                }

                TriggersCommon<T>.RaiseCommitExecuted(Database, commitExecutingContext);

                AddedAfterSaveChanges?.Invoke(addedRelationships);
                RemovedAfterSaveChanges?.Invoke(deletedRelationships);
            }
            else
            {
                Database.TriggersEnabled = false;
                await Database.SaveChangesAsync(Task.Factory.CancellationToken);
                Database.TriggersEnabled = true;
            }
        }
    }
}