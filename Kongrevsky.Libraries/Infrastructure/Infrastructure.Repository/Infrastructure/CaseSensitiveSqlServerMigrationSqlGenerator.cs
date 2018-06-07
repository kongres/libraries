namespace Kongrevsky.Infrastructure.Repository.Infrastructure
{
    #region << Using >>

    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.SqlServer;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Utilities.String;

    #endregion

    public class CaseSensitiveSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(AlterColumnOperation alterColumnOperation)
        {
            base.Generate(alterColumnOperation);
            if (alterColumnOperation.Column.Annotations.TryGetValue(nameof(CaseSensitiveAttribute).TrimEnd("Attribute"), out var values))
            {
                if (values.NewValue != null && values.NewValue.ToString() == "True")
                {
                    using (var writer = Writer())
                    {
                        //if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

                        // https://github.com/mono/entityframework/blob/master/src/EntityFramework.SqlServer/SqlServerMigrationSqlGenerator.cs
                        var columnSQL = BuildColumnType(alterColumnOperation.Column); //[nvarchar](100)
                        writer.WriteLine(
                                         "ALTER TABLE {0} ALTER COLUMN {1} {2} COLLATE SQL_Latin1_General_CP1_CS_AS {3}",
                                         alterColumnOperation.Table,
                                         alterColumnOperation.Column.Name,
                                         columnSQL,
                                         !alterColumnOperation.Column.IsNullable.HasValue || alterColumnOperation.Column.IsNullable.Value ? " NULL" : "NOT NULL" //todo not tested for DefaultValue
                                        );
                        Statement(writer);
                    }
                }
            }
        }
    }
}