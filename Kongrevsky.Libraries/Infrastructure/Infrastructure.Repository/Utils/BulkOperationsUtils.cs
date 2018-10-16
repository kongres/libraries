namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Kongrevsky.Utilities.Expression;
    using Kongrevsky.Utilities.Object;
    using SqlBulkTools;
    using SqlBulkTools.BulkCopy;

    #endregion

    internal static class BulkOperationsUtils
    {
        public static BulkAddColumnList<T> DetectColumnWithCustomColumnName<T>(this BulkAddColumnList<T> bulk)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var attrs = property.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (!attrs.Any())
                    continue;

                var columnAttribute = attrs.First() as ColumnAttribute;
                bulk.CustomColumnMapping(ExpressionUtils.ToLambda<T, object>(property.Name), columnAttribute.Name);
            }

            return bulk;
        }

        public static BulkAddColumnList<T> RemoveNotMappedColumns<T>(this BulkAddColumnList<T> bulk)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var attrs = property.GetCustomAttributes(typeof(NotMappedAttribute), true);
                if (!attrs.Any())
                    continue;

                // todo: need to recheck and refactor this code
                var columnName = ExpressionUtils.ToLambda<T, object>(property.Name);
                if (!bulk.ContainsColumns(columnName))
                    bulk.RemoveColumn(columnName);
            }

            return bulk;
        }

        public static bool ContainsColumns<T>(this AbstractColumnSelection<T> bulk, Expression<Func<T, object>> columnName)
        {
            var propertyName = GetPropertyName(columnName);
            var prop = typeof(AbstractColumnSelection<T>).GetProperty("_column", BindingFlags.NonPublic | BindingFlags.Instance);
            var getter = prop.GetGetMethod(nonPublic: true);
            var _columns = (HashSet<string>)getter.Invoke(bulk, null);

            return _columns.Contains(propertyName);
        }

        /// <summary>
        /// Original <see cref="BulkOperationsHelper.GetPropertyName"/>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetPropertyName(Expression method)
        {
            LambdaExpression lambdaExpression = method as LambdaExpression;
            if (lambdaExpression == null)
                throw new ArgumentNullException(nameof(method));
            MemberExpression memberExpression = (MemberExpression)null;
            if (lambdaExpression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)lambdaExpression.Body).Operand as MemberExpression;
                if ((memberExpression != null ? memberExpression.Expression.Type.GetCustomAttribute(typeof(ComplexTypeAttribute)) : (Attribute)null) != null && memberExpression.Expression is MemberExpression)
                    return string.Format("{0}_{1}", (object)((MemberExpression)memberExpression.Expression).Member.Name, (object)memberExpression.Member.Name);
            }
            else if (lambdaExpression.Body.NodeType == ExpressionType.MemberAccess)
                memberExpression = lambdaExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(nameof(method));
            return memberExpression.Member.Name;
        }
    }
}