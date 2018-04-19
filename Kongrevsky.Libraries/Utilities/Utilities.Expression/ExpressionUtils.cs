namespace Utilities.Expression
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class ExpressionUtils
    {
        public static string ToStringForOrdering<T, TKey>(this Expression<Func<T, TKey>> expression)
        {
            var str = expression.Body.ToString();
            var param = expression.Parameters.First().Name;
            str = str.Replace("Convert(", "(").Replace(param + ".", "");
            return str;
        }

        public static Expression<Func<T, Target>> ToLambda<T, Target>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(Target));

            return Expression.Lambda<Func<T, Target>>(propAsObject, parameter);
        }

        public static Expression<Func<TInput, TReturn>> ConvertReturnValue<TInput, TOutput, TReturn>(this Expression<Func<TInput, TOutput>> inputExpression)
        {
            Expression convertedExpressionBody = Expression.Convert(inputExpression.Body, typeof(TReturn));

            return Expression.Lambda<Func<TInput, TReturn>>(convertedExpressionBody, inputExpression.Parameters);
        }
    }
}