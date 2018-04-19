namespace Infrastructure.Repository
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;

    public class DatabaseFactory<T> : IDatabaseFactory<T> where T : DbContext, new()
    {
        public DatabaseFactory(T dataContext)
        {
            _dataContext = dataContext;
        }

        private T _dataContext { get; set; }
        public T Get()
        {
            var context = _dataContext ?? (_dataContext = new T());

            var e = new Exception("Can't connect to DB " + typeof(T));
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (context.Database.Connection.State == ConnectionState.Open)
                        return context;

                    if (context.Database.Connection.State == ConnectionState.Closed || context.Database.Connection.State == ConnectionState.Broken)
                    {
                        context.Database.Connection.Open();
                        return context;
                    }
                }
                catch (Exception exc)
                {
                    e = exc;
                }
                Thread.Sleep(1000);
            }
            throw e;
        }

        public IQueryable<TSet> GetDbSet<TSet>() where TSet : class
        {
            return Get().Set<TSet>();
        }

        public void Dispose()
        {
            _dataContext?.Dispose();
            _dataContext = null;
        }
    }
}