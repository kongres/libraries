namespace Kongrevsky.Utilities.Expression
{
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.Reflection;

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

            Expression body = parameter;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.Property(body, member);
            }

            var propAsObject = Expression.Convert(body, typeof(Target));

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

        /// <summary>
        /// Converts expression to untyped format
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression ToExpression<TInput, TOutput>(this Expression<Func<TInput, TOutput>> expression)
        {
            var body = expression.Body;
            var @params = expression.Parameters;
            var lambdaExpression = Expression.Lambda(body, @params);
            return lambdaExpression;
        }

        public static Expression CreateMemberAccess(ParameterExpression parameter, string propertyName)
        {
            Expression body = parameter;
            Type curType = parameter.Type;
            foreach (var member in propertyName.Split('.'))
            {
                var property = curType.GetPropertyByName(member);
                if (property == null)
                    return body;
                body = Expression.MakeMemberAccess(body, property);
                curType = property.PropertyType;
            }

            return body;
        }

        public static Expression<Func<TSource, int>> DescriptionOrder<TSource, TEnum>(this Expression<Func<TSource, TEnum>> source)
                where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) throw new InvalidOperationException();

            var body = ((TEnum[])Enum.GetValues(enumType))
                    .OrderBy(value => value.GetDisplayName())
                    .Select((value, ordinal) => new { value, ordinal })
                    .Reverse()
                    .Aggregate((Expression)null, (next, item) => next == null ? (Expression)
                                                         Expression.Constant(item.ordinal) :
                                                         Expression.Condition(
                                                                              Expression.Equal(source.Body, Expression.Constant(item.value)),
                                                                              Expression.Constant(item.ordinal),
                                                                              next));

            return Expression.Lambda<Func<TSource, int>>(body, source.Parameters[0]);
        }
    }
}