namespace Kongrevsky.Infrastructure.Repository.Infrastructure
{
    #region << Using >>

    using System.Data.Entity;
    using System.Data.Entity.SqlServer;

    #endregion

    public class KongrevskyDbContextConfig : DbConfiguration
    {
        public KongrevskyDbContextConfig()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
            SetMigrationSqlGenerator(SqlProviderServices.ProviderInvariantName, () => new CaseSensitiveSqlServerMigrationSqlGenerator());
        }

        public static void SetConfiguration()
        {
            SetConfiguration(new KongrevskyDbContextConfig());
        }
    }
}