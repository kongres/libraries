namespace Kongrevsky.Utilities.Expression
{
    #region << Using >>

    using System;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion

    public static class ExpressionUtils
    {
        /// <summary>
        /// Returns clean for ordering string from the Expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string ToStringForOrdering<T, TKey>(this Expression<Func<T, TKey>> expression)
        {
            var str = expression.Body.ToString();
            var param = expression.Parameters.First().Name;
            str = str.Replace("Convert(", "(").Replace(param + ".", "");
            return str;
        }

        /// <summary>
        /// Returns Lambda from property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Target"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, Target>> ToLambda<T, Target>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(Target));

            return Expression.Lambda<Func<T, Target>>(propAsObject, parameter);
        }

        /// <summary>
        /// Converts expression to typed format
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="inputExpression"></param>
        /// <returns></returns>
        public static Expression<Func<TInput, TReturn>> ConvertReturnValue<TInput, TOutput, TReturn>(this Expression<Func<TInput, TOutput>> inputExpression)
        {
            Expression convertedExpressionBody = Expression.Convert(inputExpression.Body, typeof(TReturn));

            return Expression.Lambda<Func<TInput, TReturn>>(convertedExpressionBody, inputExpression.Parameters);
        }
    }
}