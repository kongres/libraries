namespace Kongrevsky.Utilities.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static class RetryUtils
    {
        public static void Do(Action action, TimeSpan retryInterval, int retryCount = 5, bool isThrowException = true, Action<AggregateException> actionOnException = null)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount, isThrowException);
        }

        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 5, bool isThrowException = true, Action<AggregateException> actionOnException = null)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
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
            }

            var aggregateException = new AggregateException(exceptions);

            actionOnException?.Invoke(aggregateException);
            if (isThrowException)
                throw aggregateException;

            return default(T);
        }

        public static async Task<T> DoAsync<T>(Func<Task<T>> action, TimeSpan retryInterval, int retryCount = 5, bool isThrowException = true, Action<AggregateException> actionOnException = null)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
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
            }

            var aggregateException = new AggregateException(exceptions);

            actionOnException?.Invoke(aggregateException);
            if (isThrowException)
                throw aggregateException;

            return default(T);
        }
    }
}