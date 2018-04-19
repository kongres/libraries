using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Enumerable
{
    using Utilities.Enumerable.Models;

    public static class EnumerableUtils
    {
        public static void AddRange<T>(this ICollection<T> enumerable, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                enumerable.Add(item);
            }
        }

        public static void AddIfTrue<T>(this List<T> enumerable, bool condition, T entity)
        {
            if (condition)
                enumerable.Add(entity);
        }

        public static void AddIfTrue<T>(this List<T> enumerable, bool condition, Func<T> action)
        {
            if (condition)
                enumerable.Add(action.Invoke());
        }

        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                    .Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / chunkSize)
                    .Select(x => x.Select(v => v.Value).ToList())
                    .ToList();
        }

        public static bool IsPropertyEqual<T>(this IEnumerable<T> enumerable, Func<T, object> func)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            var firstOrDefault = list.FirstOrDefault();
            if (firstOrDefault == null)
                return true;
            var obj = func.Invoke(firstOrDefault);
            return list.All(x => obj.Equals(func.Invoke(x)));
        }

        public static T MinOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            if (enumerable.Any())
                return enumerable.Min();

            return defaultValue;
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            if (enumerable.Any())
                return enumerable.Max();

            return defaultValue;
        }

        public static R MinOrDefault<T, R>(this IEnumerable<T> sequence, Func<T, R> selector, R defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            if (enumerable.Any())
                return enumerable.Min(selector);

            return defaultValue;
        }

        public static R MaxOrDefault<T, R>(this IEnumerable<T> sequence, Func<T, R> selector, R defaultValue)
        {
            var enumerable = sequence as IList<T> ?? sequence.ToList();
            if (enumerable.Any())
                return enumerable.Max(selector);

            return defaultValue;
        }

        public static T AddAndReturn<T>(this ICollection<T> sequence, T entity)
        {
            sequence.Add(entity);
            return entity;
        }

        public static IDictionary<TKey, T> ToDictionaryIgnoreKeyDuplicates<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
            return enumerable.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.First());
        }

        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (var s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (var s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> queryable, Page page)
        {
            if (page.PageSize > 0 && page.PageNumber > 0)
                return queryable.Skip(page.Skip).Take(page.PageSize);
            return queryable;
        }

        public static int GetPageCount<T>(this IQueryable<T> queryable, int pageSize)
        {
            if (pageSize > 0)
                return (int)Math.Ceiling((double)queryable.Count() / pageSize);
            return 1;
        }

        public static int GetPageCount<T>(this IEnumerable<T> enumerable, int pageSize)
        {
            if (pageSize > 0)
                return (int)Math.Ceiling((double)enumerable.Count() / pageSize);
            return 1;
        }
    }

}
