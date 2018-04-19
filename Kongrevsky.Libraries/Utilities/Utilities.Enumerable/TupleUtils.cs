namespace Utilities.Enumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TupleUtils
    {
        public static bool IsTypes(this Tuple<object, object> tuple, Type type1, Type type2)
        {
            return tuple.Item1.GetType().BaseType == type1 && tuple.Item2.GetType().BaseType == type2 || tuple.Item1.GetType().BaseType == type2 && tuple.Item2.GetType().BaseType == type1;
        }

        public static Tuple<T1, T2> ConvertToTuple<T1, T2>(this Tuple<object, object> tuple)
        {
            if (tuple.Item1.GetType().BaseType == typeof(T1) && tuple.Item2.GetType().BaseType == typeof(T2))
                return new Tuple<T1, T2>((T1)tuple.Item1, (T2)tuple.Item2);
            if (tuple.Item1.GetType().BaseType == typeof(T2) && tuple.Item2.GetType().BaseType == typeof(T1))
                return new Tuple<T1, T2>((T1)tuple.Item2, (T2)tuple.Item1);
            return null;
        }

        public static IEnumerable<Tuple<T1, T2>> GetTuplesByTypes<T1, T2>(this IEnumerable<Tuple<object, object>> tuples)
        {
            return tuples.Where(x => x.IsTypes(typeof(T1), typeof(T2))).Select(x => x.ConvertToTuple<T1, T2>());
        }
    }
}