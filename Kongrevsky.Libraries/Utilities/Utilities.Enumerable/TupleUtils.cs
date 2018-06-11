namespace Kongrevsky.Utilities.Enumerable
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public static class TupleUtils
    {
        private static bool isTupleTypesStraight(Type item1Type, Type item2Type, Type type1, Type type2)
        {
            return item1Type == type1 && item2Type == type2 ||
                   item1Type.BaseType == type1 && item2Type == type2 ||
                   item1Type == type1 && item2Type.BaseType == type2 ||
                   item1Type.BaseType == type1 && item2Type.BaseType == type2;
        }

        private static bool isTupleTypesReverse(Type item1Type, Type item2Type, Type type1, Type type2)
        {
            return item1Type == type2 && item2Type == type1 ||
                   item1Type.BaseType == type2 && item2Type == type1 ||
                   item1Type == type2 && item2Type.BaseType == type1 ||
                   item1Type.BaseType == type2 && item2Type.BaseType == type1;
        }

        /// <summary>
        /// Detects if Tuple values are provide specified types
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <returns></returns>
        public static bool IsTypes(this Tuple<object, object> tuple, Type type1, Type type2)
        {
            var item1Type = tuple.Item1.GetType();
            var item2Type = tuple.Item2.GetType();

            return isTupleTypesStraight(item1Type, item2Type, type1, type2) ||
                   isTupleTypesReverse(item1Type, item2Type, type1, type2);

        }

        /// <summary>
        /// Converts Tuple of two Objects to typed Tuple
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Tuple<T1, T2> ConvertToTuple<T1, T2>(this Tuple<object, object> tuple)
        {
            var item1Type = tuple.Item1.GetType();
            var item2Type = tuple.Item2.GetType();

            var type1 = typeof(T1);
            var type2 = typeof(T2);

            if (isTupleTypesStraight(item1Type, item2Type, type1, type2))
                return new Tuple<T1, T2>((T1)tuple.Item1, (T2)tuple.Item2);

            if (isTupleTypesReverse(item1Type, item2Type, type1, type2))
                return new Tuple<T1, T2>((T1)tuple.Item2, (T2)tuple.Item1);

            return null;
        }

        /// <summary>
        /// Returns Tuples by specified types
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="tuples"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T1, T2>> GetTuplesByTypes<T1, T2>(this IEnumerable<Tuple<object, object>> tuples)
        {
            return tuples.Where(x => x.IsTypes(typeof(T1), typeof(T2))).Select(x => x.ConvertToTuple<T1, T2>());
        }
    }
}