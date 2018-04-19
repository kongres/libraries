namespace Infrastructure.Repository
{
    using System;
    using System.Threading.Tasks;
    using EntityFramework.Triggers;
    using Nito.AsyncEx;

    public class UnitOfWork<T> : IUnitOfWork<T> where T : DbContextWithTriggers
    {
        public UnitOfWork(IDatabaseFactory<T> databaseFactory)
        {
            _databaseFactory = databaseFactory;
            _lockObject = new object();
            _lockObjectAsync = new AsyncLock();
        }

        private object _lockObject { get; }
        private AsyncLock _lockObjectAsync { get;  }
        private T Database => _database ?? (_database = _databaseFactory.Get());
        private IDatabaseFactory<T> _databaseFactory { get; }
        private T _database { get; set; }

        public static Action<T> BeforeSaveChanges { get; set; }
        public static Action<T> AfterSaveChanges { get; set; }

        public void Commit()
        {
            lock (_lockObject)
            {
                BeforeSaveChanges?.Invoke(Database);
                Database.SaveChanges();
                AfterSaveChanges?.Invoke(Database);
                // if you get an exception when try commit changes in db you can use the following try block to catch exception
                //try
                //{
                //    Database.SaveChangesWithTriggers();
                //}
                //catch (DbEntityValidationException dbEx)
                //{
                //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                //    {
                //        foreach (var validationError in validationErrors.ValidationErrors)
                //        {
                //            // check error
                //        }
                //    }
                //}
            }
        }

        public async Task CommitAsync()
        {
            using (await _lockObjectAsync.LockAsync())
            {
                BeforeSaveChanges?.Invoke(Database);
                await Database.SaveChangesAsync(Task.Factory.CancellationToken);
                AfterSaveChanges?.Invoke(Database);
                // if you get an exception when try commit changes in db you can use the following try block to catch exception
                //try
                //{
                //    await Database.SaveChangesAsync(Task.Factory.CancellationToken);
                //}
                //catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                //{
                //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                //    {
                //        foreach (var validationError in validationErrors.ValidationErrors)
                //        {
                //            // check error
                //        }
                //    }
                //}
            }
        }
    }
}