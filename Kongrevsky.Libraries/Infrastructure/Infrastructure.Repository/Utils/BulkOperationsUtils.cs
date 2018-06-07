namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Kongrevsky.Utilities.Expression;
    using SqlBulkTools;

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
    }
}