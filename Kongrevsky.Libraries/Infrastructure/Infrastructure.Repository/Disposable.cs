namespace Infrastructure.Repository
{
    using System;

    public class Disposable : IDisposable
    {
        bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                DisposeCore();
            }

            this.isDisposed = true;
        }

        protected virtual void DisposeCore() { }
    }
}