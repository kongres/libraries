namespace Infrastructure.Repository.Utils
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using SqlBulkTools;

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
                bulk.CustomColumnMapping(Utilities.Expression.ExpressionUtils.ToLambda<T, object>(property.Name), columnAttribute.Name);
            }

            return bulk;
        }
    }
}