namespace Kongrevsky.Utilities.Common
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    public static class RetryUtils
    {
        /// <summary>
        /// Provides specified action with retry options 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval"></param>
        /// <param name="retryCount"></param>
        /// <param name="isThrowException"></param>
        /// <param name="actionOnException"></param>
        public static void Do(Action action, TimeSpan retryInterval, int retryCount = 5, bool isThrowException = true, Action<AggregateException> actionOnException = null)
        {
            Do<object>(() =>
                       {
                           action();
                           return null;
                       }, retryInterval, retryCount, isThrowException, actionOnException);
        }

        /// <summary>
        /// Provides specified action with retry options 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="retryInterval"></param>
        /// <param name="retryCount"></param>
        /// <param name="isThrowException"></param>
        /// <param name="actionOnException"></param>
        /// <returns></returns>
        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 5, bool isThrowException = true, Action<AggregateException> actionOnException = null)
        {
            var exceptions = new List<Exception>();

            for (var retry = 0; retry < retryCount; retry++)
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);

                    var result = action.Invoke();
                    return result;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

            var aggregateException = new AggregateException(exceptions);

            actionOnException?.Invoke(aggregateException);
            if (isThrowException)
                throw aggregateException;

            return default(T);
        }

        /// <summary>
        /// Async provides specified action with retry options 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="retryInterval"></param>
        /// <param name="retryCount"></param>
        /// <param name="isThrowException"></param>
        /// <param name="actionOnException"></param>
        /// <returns></returns>
        public static async Task<T> DoAsync<T>(Func<Task<T>> action, TimeSpan retryInterval, int retryCount = 5, bool isThrowException = true, Action<AggregateException> actionOnException = null)
        {
            var exceptions = new List<Exception>();

            for (var retry = 0; retry < retryCount; retry++)
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);

                    var result = action.Invoke();
                    return await result;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

            var aggregateException = new AggregateException(exceptions);

            actionOnException?.Invoke(aggregateException);
            if (isThrowException)
                throw aggregateException;

            return default(T);
        }
    }
}