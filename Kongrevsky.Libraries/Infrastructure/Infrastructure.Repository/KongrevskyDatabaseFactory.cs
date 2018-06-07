namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.Data;
    using System.Linq;
    using System.Threading;

    #endregion

    public class KongrevskyDatabaseFactory<T> : IKongrevskyDatabaseFactory<T> where T : KongrevskyDbContext
    {
        public KongrevskyDatabaseFactory(T dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        private T _dataContext { get; set; }

        public T Get()
        {
            var context = _dataContext;

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