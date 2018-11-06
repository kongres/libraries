namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Kongrevsky.Utilities.Expression;
    using SqlBulkTools;
    using System.Linq.Expressions;
    using SqlBulkTools.Enumeration;

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
                if (!string.IsNullOrEmpty(columnAttribute?.Name))
                {
                    bulk.CustomColumnMapping(ExpressionUtils.ToLambda<T, object>(property.Name), columnAttribute.Name);
                }
            }

            return bulk;
        }

        public static BulkInsert<T> ApplyIdentityColumn<T>(this BulkInsert<T> bulk, Expression<Func<T, object>> identificator, ColumnDirectionType columnDirectionType)
        {
            return identificator != null ? bulk.SetIdentityColumn(identificator, columnDirectionType) : bulk;
        }

        public static BulkAddColumnList<T> RemoveNotMappedColumns<T>(this BulkAddColumnList<T> bulk)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var attrs = property.GetCustomAttributes(typeof(NotMappedAttribute), true);
                if (!attrs.Any())
                    continue;

                var columnName = ExpressionUtils.ToLambda<T, object>(property.Name);

                try
                {
                    bulk.RemoveColumn(columnName);
                }
                catch (Exception e)
                {
                }
            }

            return bulk;
        }
    }
}