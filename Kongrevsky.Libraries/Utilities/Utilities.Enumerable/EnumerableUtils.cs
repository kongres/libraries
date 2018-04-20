namespace Utilities.Enumerable
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities.Enumerable.Models;

    #endregion

    public static class EnumerableUtils
    {
        /// <summary>
        /// Add range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="range"></param>
        public static void AddRange<T>(this ICollection<T> enumerable, IEnumerable<T> range)
        {
            foreach (var item in range)
                enumerable.Add(item);
        }

        /// <summary>
        /// Adds value to IList if condition is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="condition"></param>
        /// <param name="entity"></param>
        public static void AddIfTrue<T>(this IList<T> enumerable, bool condition, T entity)
        {
            if (condition)
                enumerable.Add(entity);
        }

        /// <summary>
        /// Adds value to IList if condition is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        public static void AddIfTrue<T>(this IList<T> enumerable, bool condition, Func<T> action)
        {
            if (condition)
                enumerable.Add(action.Invoke());
        }

        /// <summary>
        /// Returns List of chunks by specified chunk size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                   .Select((x, i) => new { Index = i, Value = x })
                   .GroupBy(x => x.Index / chunkSize)
                   .Select(x => x.Select(v => v.Value).ToList())
                   .ToList();
        }

        /// <summary>
        /// Returns true if all values are equal to specified Func result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool IsPropertyEqual<T>(this IEnumerable<T> enumerable, Func<T, object> func)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            var firstOrDefault = list.FirstOrDefault();
            if (firstOrDefault == null)
                return true;

            var obj = func.Invoke(firstOrDefault);
            return list.All(x => obj.Equals(func.Invoke(x)));
        }

        /// <summary>
        /// Returns Min value of the sequence or specified default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T MinOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();

            return enumerable.Any() ?
                           enumerable.Min() :
                           defaultValue;
        }

        /// <summary>
        /// Returns Max value of the sequence or specified default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T MaxOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            return enumerable.Any() ?
                           enumerable.Max() :
                           defaultValue;
        }

        /// <summary>
        /// Returns Min value of the sequence or specified default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static R MinOrDefault<T, R>(this IEnumerable<T> sequence, Func<T, R> selector, R defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            return enumerable.Any() ?
                           enumerable.Min(selector) :
                           defaultValue;
        }

        /// <summary>
        /// Returns Max value of the sequence or specified default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static R MaxOrDefault<T, R>(this IEnumerable<T> sequence, Func<T, R> selector, R defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            return enumerable.Any() ?
                           enumerable.Max(selector) :
                           defaultValue;
        }

        /// <summary>
        /// Adds and returns Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T AddAndReturn<T>(this ICollection<T> sequence, T entity)
        {
            sequence.Add(entity);
            return entity;
        }

        /// <summary>
        /// Returns only first duplicated values or/and non-duplicated values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IDictionary<TKey, T> ToDictionaryIgnoreKeyDuplicates<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
            return enumerable.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.First());
        }

        /// <summary>
        /// Detects if IEnumerables are full-equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (var s in list1)
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else
                    cnt.Add(s, 1);

            foreach (var s in list2)
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else
                    return false;

            return cnt.Values.All(c => c == 0);
        }

        /// <summary>
        /// Returns HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        /// <summary>
        /// Returns Page of the IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> queryable, Page page)
        {
            if (page.PageSize > 0 && page.PageNumber > 0)
                return queryable.Skip(page.Skip).Take(page.PageSize);

            return queryable;
        }

        /// <summary>
        /// Returns Page of the IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IQueryable<T> GetPage<T>(this IQueryable<T> queryable, Page page)
        {
            if (page.PageSize > 0 && page.PageNumber > 0)
                return queryable.Skip(page.Skip).Take(page.PageSize);

            return queryable;
        }

        /// <summary>
        /// Returns Page count of the IQueryable by specified page size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int GetPageCount<T>(this IQueryable<T> queryable, int pageSize)
        {
            if (pageSize > 0)
                return (int)Math.Ceiling((double)queryable.Count() / pageSize);

            return 1;
        }

        /// <summary>
        /// Returns Page count of the IEnumerable by specified page size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int GetPageCount<T>(this IEnumerable<T> enumerable, int pageSize)
        {
            if (pageSize > 0)
                return (int)Math.Ceiling((double)enumerable.Count() / pageSize);

            return 1;
        }
    }
}