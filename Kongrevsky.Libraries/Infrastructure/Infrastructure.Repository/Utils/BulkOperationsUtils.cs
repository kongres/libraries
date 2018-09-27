namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Kongrevsky.Utilities.Expression;
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

                bulk.RemoveColumn(ExpressionUtils.ToLambda<T, object>(property.Name));
            }

            return bulk;
        }
    }
}