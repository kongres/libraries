namespace Kongrevsky.Infrastructure.Repository.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity.SqlServer;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Kongrevsky.Utilities.Expression;
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.Reflection;
    using LinqKit;

    internal static class QueryableUtils
    {
        public static IOrderedQueryable<T> OrderByDescendingWithNullLowPriority<T, Target>(this IQueryable<T> queryable, Expression<Func<T, Target>> expression)
        {
            if (typeof(Target) == typeof(string))
            {
                var strExpr = expression.ConvertReturnValue<T, Target, string>();
                return queryable.OrderByDescending(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenByDescending(expression);
            }
            else if (!typeof(Target).IsNullable())
                return queryable.OrderByDescending(expression);
            else
                return queryable.OrderByDescending(x => expression.Invoke(x) == null).ThenByDescending(expression);
        }

        public static IOrderedQueryable<T> OrderByWithNullLowPriority<T, Target>(this IQueryable<T> queryable, Expression<Func<T, Target>> expression)
        {
            if (typeof(Target) == typeof(string))
            {
                var strExpr = expression.ConvertReturnValue<T, Target, string>();
                return queryable.OrderBy(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenBy(expression);
            }
            else if (!typeof(Target).IsNullable())
                return queryable.OrderBy(expression);
            else
                return queryable.OrderBy(x => expression.Invoke(x) == null).ThenBy(expression);
        }

        public static IOrderedQueryable<T> ThenByDescendingWithNullLowPriority<T, Target>(this IOrderedQueryable<T> queryable, Expression<Func<T, Target>> expression)
        {
            if (typeof(Target) == typeof(string))
            {
                var strExpr = expression.ConvertReturnValue<T, Target, string>();
                return queryable.ThenByDescending(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenByDescending(expression);
            }
            else if (!typeof(Target).IsNullable())
                return queryable.ThenByDescending(expression);
            else
                return queryable.ThenByDescending(x => expression.Invoke(x) == null).ThenByDescending(expression);
        }

        public static IOrderedQueryable<T> ThenByWithNullLowPriority<T, Target>(this IOrderedQueryable<T> queryable, Expression<Func<T, Target>> expression)
        {
            if (typeof(Target) == typeof(string))
            {
                var strExpr = expression.ConvertReturnValue<T, Target, string>();
                return queryable.ThenBy(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenBy(expression);
            }
            else if (!typeof(Target).IsNullable())
                return queryable.ThenBy(expression);
            else
                return queryable.ThenBy(x => expression.Invoke(x) == null).ThenBy(expression);
        }

        public static IOrderedQueryable<T> OrderByWithNullLowPriority<T>(this IQueryable<T> queryable, string propertyName = null, bool isDesc = false)
        {
            if (isDesc)
                return queryable.OrderByDescendingWithNullLowPriority(propertyName);

            var type = typeof(T);
            var property = type.GetProperties().FirstOrDefault(x => !string.IsNullOrEmpty(propertyName) && string.Equals(x.Name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                           ?? type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true).Any());

            if (property == null)
                return queryable.OrderBy(x => true);

            if (property.PropertyType == typeof(string))
            {
                var strExpr = ExpressionUtils.ToLambda<T, string>(property.Name);
                return queryable.OrderBy(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenBy(strExpr);
            }
            if (property.PropertyType.IsEnum)
            {
                var enumType = property.PropertyType;
                var strExpr = ExpressionUtils.ToLambda<T, int>(property.Name);

                var enumValues = Enum.GetValues(enumType);
                var objs = enumValues.Cast<object>().Select((t, i) => enumValues.GetValue(i)).ToList();
                var body = objs
                        .OrderBy(value => value.GetDisplayName())
                        .Select((value, ordinal) => new { value = (int)value, ordinal })
                        .Reverse()
                        .Aggregate((Expression)null,
                                   (next, item) => next == null
                                       ? (Expression)
                                       Expression.Constant(item.ordinal)
                                       : Expression.Condition(Expression.Equal(strExpr.Body, Expression.Constant(item.value)), Expression.Constant(item.ordinal), next));
                var expr = Expression.Lambda<Func<T, int>>(body, strExpr.Parameters[0]);
                return queryable.OrderBy(expr);
            }
            if (property.PropertyType.IsGenericType && property.PropertyType.GenericTypeArguments[0].IsEnum)
            {
                var enumType = property.PropertyType.GenericTypeArguments[0];
                var strExpr = ExpressionUtils.ToLambda<T, int>(property.Name);

                var enumValues = Enum.GetValues(enumType);
                var objs = enumValues.Cast<object>().Select((t, i) => enumValues.GetValue(i)).ToList();
                var body = objs
                        .OrderBy(value => value.GetDisplayName())
                        .Select((value, ordinal) => new { value = (int)value, ordinal })
                        .Reverse()
                        .Aggregate((Expression)null,
                                   (next, item) => next == null
                                       ? (Expression)
                                       Expression.Constant(item.ordinal)
                                       : Expression.Condition(Expression.Equal(strExpr.Body, Expression.Constant(item.value)), Expression.Constant(item.ordinal), next));
                var expr = Expression.Lambda<Func<T, int>>(body, strExpr.Parameters[0]);
                return queryable.OrderBy(x => strExpr.Invoke(x) == null).ThenBy(expr);
            }
            if (!property.PropertyType.IsNullable())
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(typeof(Queryable), "OrderBy", new[] { type, property.PropertyType }, queryable.Expression, Expression.Quote(orderByExp));
                return (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(resultExp);
            }
            else
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                var orderByExp = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyAccess, Expression.Constant(null, property.PropertyType)), parameter);
                var orderBy = Expression.Call(typeof(Queryable), "OrderBy", new[] { type, orderByExp.Body.Type }, queryable.Expression, Expression.Quote(orderByExp));

                var orderedQueryable = (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(orderBy);

                var thenByExp = Expression.Lambda(propertyAccess, parameter);
                var thenBy = Expression.Call(typeof(Queryable), "ThenBy", new[] { type, property.PropertyType }, orderedQueryable.Expression, Expression.Quote(thenByExp));

                return (IOrderedQueryable<T>)orderedQueryable.Provider.CreateQuery<T>(thenBy);
            }
        }

        public static IOrderedQueryable<T> OrderByDescendingWithNullLowPriority<T>(this IQueryable<T> queryable, string propertyName = null)
        {
            var type = typeof(T);
            var property = type.GetProperties().FirstOrDefault(x => !string.IsNullOrEmpty(propertyName) && string.Equals(x.Name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                           ?? type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true).Any());

            if (property == null)
                return queryable.OrderByDescending(x => true);

            if (property.PropertyType == typeof(string))
            {
                var strExpr = ExpressionUtils.ToLambda<T, string>(property.Name);
                return queryable.OrderByDescending(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenByDescending(strExpr);
            }
            if (property.PropertyType.IsEnum)
            {
                var enumType = property.PropertyType;
                var strExpr = ExpressionUtils.ToLambda<T, int>(property.Name);

                var enumValues = Enum.GetValues(enumType);
                var objs = enumValues.Cast<object>().Select((t, i) => enumValues.GetValue(i)).ToList();
                var body = objs
                        .OrderBy(value => value.GetDisplayName())
                        .Select((value, ordinal) => new { value = (int)value, ordinal })
                        .Reverse()
                        .Aggregate((Expression)null,
                                   (next, item) => next == null
                                       ? (Expression)
                                       Expression.Constant(item.ordinal)
                                       : Expression.Condition(Expression.Equal(strExpr.Body, Expression.Constant(item.value)), Expression.Constant(item.ordinal), next));
                var expr = Expression.Lambda<Func<T, int>>(body, strExpr.Parameters[0]);
                return queryable.OrderByDescending(expr);
            }
            if (property.PropertyType.IsGenericType && property.PropertyType.GenericTypeArguments[0].IsEnum)
            {
                var enumType = property.PropertyType.GenericTypeArguments[0];
                var strExpr = ExpressionUtils.ToLambda<T, int>(property.Name);

                var enumValues = Enum.GetValues(enumType);
                var objs = enumValues.Cast<object>().Select((t, i) => enumValues.GetValue(i)).ToList();
                var body = objs
                        .OrderBy(value => value.GetDisplayName())
                        .Select((value, ordinal) => new { value = (int)value, ordinal })
                        .Reverse()
                        .Aggregate((Expression)null,
                                   (next, item) => next == null
                                       ? (Expression)
                                       Expression.Constant(item.ordinal)
                                       : Expression.Condition(Expression.Equal(strExpr.Body, Expression.Constant(item.value)), Expression.Constant(item.ordinal), next));
                var expr = Expression.Lambda<Func<T, int>>(body, strExpr.Parameters[0]);
                return queryable.OrderByDescending(x => strExpr.Invoke(x) == null).ThenByDescending(expr);
            }
            if (!property.PropertyType.IsNullable())
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { type, property.PropertyType }, queryable.Expression, Expression.Quote(orderByExp));
                return (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(resultExp);
            }
            else
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                var orderByDescendingExp = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyAccess, Expression.Constant(null, property.PropertyType)), parameter);
                var orderByDescending = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { type, orderByDescendingExp.Body.Type }, queryable.Expression, Expression.Quote(orderByDescendingExp));

                var orderedQueryable = (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(orderByDescending);

                var thenByDescendingExp = Expression.Lambda(propertyAccess, parameter);
                var thenByDescending = Expression.Call(typeof(Queryable), "ThenByDescending", new[] { type, property.PropertyType }, orderedQueryable.Expression, Expression.Quote(thenByDescendingExp));

                return (IOrderedQueryable<T>)orderedQueryable.Provider.CreateQuery<T>(thenByDescending);
            }
        }

        public static IOrderedQueryable<T> ThenByWithNullLowPriority<T>(this IOrderedQueryable<T> queryable, string propertyName = null)
        {
            var type = typeof(T);
            var property = type.GetProperties().FirstOrDefault(x => !string.IsNullOrEmpty(propertyName) && string.Equals(x.Name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                           ?? type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true).Any());
            if (property == null)
                return queryable.ThenBy(x => true);

            if (property.PropertyType == typeof(string))
            {
                var strExpr = ExpressionUtils.ToLambda<T, string>(property.Name);
                return queryable.ThenBy(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenBy(strExpr);
            }
            if (!property.PropertyType.IsNullable())
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(typeof(Queryable), "ThenBy", new[] { type, property.PropertyType }, queryable.Expression, Expression.Quote(orderByExp));
                return (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(resultExp);
            }
            else
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                var orderByExp = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyAccess, Expression.Constant(null, property.PropertyType)), parameter);
                var orderBy = Expression.Call(typeof(Queryable), "ThenBy", new[] { type, orderByExp.Body.Type }, queryable.Expression, Expression.Quote(orderByExp));

                var orderedQueryable = (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(orderBy);

                var thenByExp = Expression.Lambda(propertyAccess, parameter);
                var thenBy = Expression.Call(typeof(Queryable), "ThenBy", new[] { type, property.PropertyType }, orderedQueryable.Expression, Expression.Quote(thenByExp));

                return (IOrderedQueryable<T>)orderedQueryable.Provider.CreateQuery<T>(thenBy);
            }
        }

        public static IOrderedQueryable<T> ThenByDescendingWithNullLowPriority<T>(this IOrderedQueryable<T> queryable, string propertyName = null)
        {
            var type = typeof(T);
            var property = type.GetProperties().FirstOrDefault(x => !string.IsNullOrEmpty(propertyName) && string.Equals(x.Name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                           ?? type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(DefaultSortPropertyAttribute), true).Any());

            if (property == null)
                return queryable.ThenByDescending(x => true);

            if (property.PropertyType == typeof(string))
            {
                var strExpr = ExpressionUtils.ToLambda<T, string>(property.Name);
                return queryable.ThenByDescending(x => string.IsNullOrEmpty(strExpr.Invoke(x))).ThenByDescending(strExpr);
            }
            if (!property.PropertyType.IsNullable())
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(typeof(Queryable), "ThenByDescending", new[] { type, property.PropertyType }, queryable.Expression, Expression.Quote(orderByExp));
                return (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(resultExp);
            }
            else
            {
                var parameter = Expression.Parameter(type);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                var orderByDescendingExp = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyAccess, Expression.Constant(null, property.PropertyType)), parameter);
                var orderByDescending = Expression.Call(typeof(Queryable), "ThenByDescending", new[] { type, orderByDescendingExp.Body.Type }, queryable.Expression, Expression.Quote(orderByDescendingExp));

                var orderedQueryable = (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(orderByDescending);

                var thenByDescendingExp = Expression.Lambda(propertyAccess, parameter);
                var thenByDescending = Expression.Call(typeof(Queryable), "ThenByDescending", new[] { type, property.PropertyType }, orderedQueryable.Expression, Expression.Quote(thenByDescendingExp));

                return (IOrderedQueryable<T>)orderedQueryable.Provider.CreateQuery<T>(thenByDescending);
            }
        }

        public static IQueryable<T> DistinctByField<T>(this IQueryable<T> queryable, string propertyName)
        {
            var type = typeof(T);
            var property = type.GetPropertyByName(propertyName);
            if (property == null)
                return queryable;

            var parameter = Expression.Parameter(type);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var groupByExp = Expression.Lambda(propertyAccess, parameter);
            var groupByQueryable = queryable.Provider.CreateQuery(Expression.Call(typeof(Queryable), "GroupBy", new[] { type, property.PropertyType }, queryable.Expression, Expression.Quote(groupByExp)));

            var groupByArgument = typeof(IGrouping<,>).MakeGenericType(property.PropertyType, type);

            var parameterExpression = Expression.Parameter(groupByArgument, "x");
            var firstOrDefaultMethod = typeof(Enumerable).GetMethods().FirstOrDefault(x => x.Name == "FirstOrDefault");
            var firstOrDefaultMethodExp = Expression.Lambda(Expression.Call(null, firstOrDefaultMethod.MakeGenericMethod(type), parameterExpression), parameterExpression);

            var selectMethod = typeof(Queryable).GetMethods().FirstOrDefault(x => x.Name == "Select");
            var selectQueryable = groupByQueryable.Provider.CreateQuery<T>(Expression.Call(null, selectMethod.MakeGenericMethod(groupByArgument, type), groupByQueryable.Expression, firstOrDefaultMethodExp));

            return selectQueryable;
        }

        public static Expression<Func<T, bool>> FiltersToLambda<T>(List<string> conditions)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "x");

            var conditionKeyPairs = ToPropertyValuePairs(conditions, type);

            if (!conditionKeyPairs.Any())
                return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameter);


            Expression GetExpression(PropertyInfo property, string value)
            {
                var expProp = Expression.Property(parameter, property.Name);

                if (string.IsNullOrEmpty(value) && property.PropertyType.IsNullable())
                {
                    if (property.PropertyType == typeof(string))
                        return Expression.Call(typeof(string), nameof(string.IsNullOrEmpty), null, expProp);
                    if (property.PropertyType.GetInterfaces().Any(x => x == typeof(IEnumerable)) && property.PropertyType.GetGenericArguments().FirstOrDefault() == typeof(ItemDto))
                    {
                        var countMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().FirstOrDefault(m => m.Name == "Count" && m.GetParameters().Count() == 1)?.MakeGenericMethod(typeof(ItemDto));
                        if (countMethod == null)
                            return Expression.Constant(true);
                        return Expression.Equal(Expression.Call(null, countMethod, expProp), Expression.Constant(0));
                    }
                    return Expression.Equal(expProp, Expression.Constant(null, property.PropertyType));
                }

                if (property.PropertyType == typeof(string))
                {
                    return Expression.Call(expProp, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }), Expression.Constant(value, typeof(string)));
                }
                if (property.PropertyType == typeof(DateTime))
                {
                    if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset intValueOffset))
                        return Expression.Constant(true);
                    var intValue = intValueOffset.DateTime;
                    var dateDiff = typeof(SqlFunctions).GetMethod(nameof(SqlFunctions.DateDiff), new[] { typeof(string), typeof(DateTime?), typeof(DateTime?) });
                    var day = intValue == intValue.Date ? "DAY" : "MINUTE";
                    var callDateDiff = Expression.Call(null, dateDiff, Expression.Constant(day), Expression.Convert(expProp, typeof(DateTime?)), Expression.Convert(Expression.Constant(intValue), typeof(DateTime?)));
                    return Expression.Equal(callDateDiff, Expression.Convert(Expression.Constant(0), typeof(int?)));
                }
                if (property.PropertyType == typeof(DateTime?))
                {
                    if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset intValueOffset))
                        return Expression.Constant(true);
                    var intValue = intValueOffset.DateTime;
                    var hasValue = Expression.Property(expProp, nameof(Nullable<DateTime>.HasValue));
                    var dateDiff = typeof(SqlFunctions).GetMethod(nameof(SqlFunctions.DateDiff), new[] { typeof(string), typeof(DateTime?), typeof(DateTime?) });
                    var day = intValue == intValue.Date ? "DAY" : "MINUTE";
                    var callDateDiff = Expression.Call(null, dateDiff, Expression.Constant(day), Expression.Convert(expProp, typeof(DateTime?)), Expression.Convert(Expression.Constant(intValue), typeof(DateTime?)));
                    return Expression.And(hasValue, Expression.Equal(callDateDiff, Expression.Convert(Expression.Constant(0), typeof(int?))));
                }
                if (property.PropertyType == typeof(int))
                {
                    if (!value.TryConvertToType(out int intValue))
                        return Expression.Constant(true);
                    return Expression.Equal(expProp, Expression.Constant(intValue));
                }
                if (property.PropertyType == typeof(int?))
                {
                    if (!value.TryConvertToType(out int intValue))
                        return Expression.Constant(true);
                    var memberExpression = expProp;
                    return Expression.And(Expression.Property(memberExpression, nameof(Nullable<double>.HasValue)), Expression.Equal(Expression.Property(memberExpression, nameof(Nullable<double>.Value)), Expression.Constant(intValue)));
                }
                if (property.PropertyType == typeof(double))
                {
                    if (!value.TryConvertToType(out double intValue))
                        return Expression.Constant(true);
                    var methodInfo = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(double) });
                    var memberExpression = expProp;
                    return Expression.LessThanOrEqual(Expression.Call(null, methodInfo, Expression.Subtract(memberExpression, Expression.Constant(intValue))), Expression.Constant((double)0.1));
                }
                if (property.PropertyType == typeof(double?))
                {
                    if (!value.TryConvertToType(out double intValue))
                        return Expression.Constant(true);
                    var methodInfo = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(double) });
                    var memberExpression = expProp;
                    return Expression.And(Expression.Property(memberExpression, nameof(Nullable<double>.HasValue)), Expression.LessThanOrEqual(Expression.Call(null, methodInfo, Expression.Subtract(Expression.Property(memberExpression, nameof(Nullable<double>.Value)), Expression.Constant(intValue))), Expression.Constant((double)0.1)));
                }
                if (property.PropertyType == typeof(decimal))
                {
                    if (!value.TryConvertToType(out decimal intValue))
                        return Expression.Constant(true);
                    var methodInfo = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(decimal) });
                    var memberExpression = expProp;
                    return Expression.LessThanOrEqual(Expression.Call(null, methodInfo, Expression.Subtract(memberExpression, Expression.Constant(intValue))), Expression.Constant((decimal)0.1));
                }
                if (property.PropertyType == typeof(decimal?))
                {
                    if (!value.TryConvertToType(out decimal intValue))
                        return Expression.Constant(true);
                    var methodInfo = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(decimal) });
                    var memberExpression = expProp;
                    return Expression.And(Expression.Property(memberExpression, nameof(Nullable<decimal>.HasValue)), Expression.LessThanOrEqual(Expression.Call(null, methodInfo, Expression.Subtract(Expression.Property(memberExpression, nameof(Nullable<decimal>.Value)), Expression.Constant(intValue))), Expression.Constant((decimal)0.1)));
                }
                if (property.PropertyType == typeof(float))
                {
                    if (!value.TryConvertToType(out float intValue))
                        return Expression.Constant(true);
                    var methodInfo = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(float) });
                    var memberExpression = expProp;
                    return Expression.LessThanOrEqual(Expression.Call(null, methodInfo, Expression.Subtract(memberExpression, Expression.Constant(intValue))), Expression.Constant((float)0.1));
                }
                if (property.PropertyType == typeof(float?))
                {
                    if (!value.TryConvertToType(out float intValue))
                        return Expression.Constant(true);
                    var methodInfo = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(float) });
                    var memberExpression = expProp;
                    return Expression.And(Expression.Property(memberExpression, nameof(Nullable<float>.HasValue)), Expression.LessThanOrEqual(Expression.Call(null, methodInfo, Expression.Subtract(Expression.Property(memberExpression, nameof(Nullable<float>.Value)), Expression.Constant(intValue))), Expression.Constant((float)0.1)));
                }
                if (property.PropertyType == typeof(bool))
                {
                    if (!value.TryConvertToType(out bool intValue))
                        return Expression.Constant(true);
                    return Expression.Equal(expProp, Expression.Constant(intValue));
                }
                if (property.PropertyType == typeof(bool?))
                {
                    if (!value.TryConvertToType(out bool intValue))
                        return Expression.Constant(true);
                    return Expression.And(Expression.Property(expProp, nameof(Nullable<bool>.HasValue)), Expression.Equal(Expression.Property(expProp, nameof(Nullable<bool>.Value)), Expression.Constant(intValue)));
                }
                if (property.PropertyType.GetInterfaces().Any(x => x == typeof(IEnumerable)) && property.PropertyType.GetGenericArguments().FirstOrDefault() == typeof(ItemDto))
                {
                    var parameterExpression = Expression.Parameter(typeof(ItemDto), "i");
                    var containsMethod = Expression.Call(Expression.Property(parameterExpression, nameof(ItemDto.Name)), typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }), Expression.Constant(value, typeof(string)));
                    var anyExpr = Expression.Lambda<Func<ItemDto, bool>>(containsMethod, parameterExpression);
                    var anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().FirstOrDefault(m => m.Name == "Any" && m.GetParameters().Count() == 2)?.MakeGenericMethod(typeof(ItemDto));
                    if (anyMethod == null)
                        return Expression.Constant(true);
                    return Expression.Call(null, anyMethod, expProp, anyExpr);
                }
                if (property.PropertyType.IsGenericType && (property.PropertyType.GetGenericArguments().FirstOrDefault()?.IsEnum ?? false))
                {
                    if (value.TryConvertToType(property.PropertyType.GetGenericArguments().FirstOrDefault(), out var enumValue))
                        return Expression.And(Expression.Property(expProp, nameof(Nullable<bool>.HasValue)), Expression.Equal(Expression.Property(expProp, nameof(Nullable<bool>.Value)), Expression.Constant(enumValue)));
                    return Expression.Constant(true);
                }
                if (property.PropertyType.IsEnum)
                {
                    if (value.TryConvertToType(property.PropertyType, out var enumValue))
                        return Expression.Equal(expProp, Expression.Constant(enumValue));
                    return Expression.Constant(true);
                }

                return Expression.Constant(true);
            }

            var expressions = conditionKeyPairs.Select(x => x.Select(c => GetExpression(c.Property, c.Value)));
            var result = expressions.Select(x => x.Aggregate(Expression.Or)).Aggregate(Expression.And);
            return Expression.Lambda<Func<T, bool>>(result, parameter);
        }

        public static List<List<PropertyFilterItem>> ToPropertyValuePairs(List<string> conditions, Type type)
        {
            const string separatorEqual = "==";
            const string separatorFullEqual = "===";
            return conditions
                    .Where(x => x.Contains(separatorEqual))
                    .Select(x =>
                    {
                        var oneRow = x.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                        return oneRow.Select(c =>
                        {
                            string[] cond;
                            string separator;
                            if (c.Contains(separatorFullEqual))
                            {
                                separator = separatorFullEqual;
                                cond = c.Split(new[] { separatorFullEqual }, StringSplitOptions.None);
                            }
                            else if (c.Contains(separatorEqual))
                            {
                                separator = separatorEqual;
                                cond = c.Split(new[] { separatorEqual }, StringSplitOptions.None);
                            }
                            else
                            {
                                return null;
                            }

                            var propertyName = cond.ElementAtOrDefault(0);
                            var property = type.GetProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
                            var value = string.Join(separator, cond.Skip(1));
                            var propertyFilterItem = new PropertyFilterItem()
                            {
                                Property = property,
                                Value = value
                            };
                            switch (separator)
                            {
                                case separatorEqual:
                                    propertyFilterItem.Operation = OperationFilter.Equal;
                                    break;
                                case separatorFullEqual:
                                    propertyFilterItem.Operation = OperationFilter.FullEqual;
                                    break;
                            }
                            return propertyFilterItem;
                        })
                                .Where(c => c?.Property != null)
                                .ToList();
                    })
                    .Where(x => x.Any())
                    .ToList();
        }

        public class PropertyFilterItem
        {
            public PropertyFilterItem()
            {
                Operation = OperationFilter.Equal;
            }

            public PropertyInfo Property { get; set; }
            public OperationFilter Operation { get; set; }
            public string Value { get; set; }
        }

        public enum OperationFilter
        {
            Equal,
            FullEqual
        }
    }

}