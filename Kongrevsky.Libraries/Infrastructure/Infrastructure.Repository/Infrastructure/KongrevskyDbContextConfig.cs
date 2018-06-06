namespace Kongrevsky.Infrastructure.Repository.Infrastructure
{
    using System.Data.Entity;
    using System.Data.Entity.SqlServer;

    public class KongrevskyDbContextConfig : DbConfiguration
    {
        public KongrevskyDbContextConfig()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
            SetMigrationSqlGenerator(SqlProviderServices.ProviderInvariantName, () => new CaseSensitiveSqlServerMigrationSqlGenerator());
        }

        public static void SetConfiguration()
        {
            DbConfiguration.SetConfiguration(new KongrevskyDbContextConfig());
        }
    }
}