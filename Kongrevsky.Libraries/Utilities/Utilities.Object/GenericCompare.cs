namespace Utilities.Object
{
    #region << Using >>

    using System;
    using System.Collections.Generic;

    #endregion

    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        #region Properties

        private Func<T, object> _expr { get; set; }

        #endregion

        #region Constructors

        public GenericCompare(Func<T, object> expr)
        {
            _expr = expr;
        }

        #endregion

        #region Interface Implementations

        public bool Equals(T x, T y)
        {
            var first = _expr.Invoke(x);
            var sec = _expr.Invoke(y);
            if (first != null && first.Equals(sec))
                return true;
            else
                return false;
        }

        public int GetHashCode(T obj)
        {
            var value = _expr.Invoke(obj);
            return value.GetHashCode();
        }

        #endregion
    }
}