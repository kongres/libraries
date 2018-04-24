namespace Kongrevsky.Infrastructure.Repository
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;
    using EntityFramework.Triggers;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Infrastructure.Repository.Infrastructure;
    using Kongrevsky.Utilities.Object;

    public class KongrevskyDbContext : DbContextWithTriggers
    {
        public string ConnectionString { get; set; }

        public KongrevskyDbContext(string connectionString)
                : base(connectionString)
        {
            ConnectionString = connectionString;
            Configure();
        }

        private void Configure()
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += (sender, args) => ObjectUtils.FixDates(args.Entity);
            Database.CommandTimeout = 180;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 4));

            modelBuilder.Conventions.Add(new NonPublicColumnAttributeConvention());
            modelBuilder.Conventions.Add(new AttributeToColumnAnnotationConvention<CaseSensitiveAttribute, bool>("CaseSensitive", (property, attributes) => attributes.Single().IsEnabled));

            base.OnModelCreating(modelBuilder);
        }
    }
}