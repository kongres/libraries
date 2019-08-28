namespace Kongrevsky.Infrastructure.LogManager.Infrastructure
{
    #region << Using >>

    using System.Data.Entity;
    using Kongrevsky.Infrastructure.LogManager.Models;
    using Kongrevsky.Infrastructure.Repository;

    #endregion

    public class LogDbContext : KongrevskyDbContext
    {
        static LogDbContext()
        {
            Database.SetInitializer(new LogDbInitializer());
        }

        public LogDbContext(string connectionString)
                : base(connectionString) { }


        public DbSet<LogItem> LogItems { get; set; }

        public DbSet<AdditionalInfoLogItem> AdditionalInfoLogItems { get; set; }
    }

    public class LogDbInitializer : CreateDatabaseIfNotExists<LogDbContext> { }
}