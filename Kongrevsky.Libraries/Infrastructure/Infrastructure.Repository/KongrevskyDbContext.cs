namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;
    using EntityFramework.Triggers;
    using Kongrevsky.Infrastructure.Repository.Attributes;
    using Kongrevsky.Infrastructure.Repository.Infrastructure;
    using Kongrevsky.Utilities.Object;

    #endregion

    public class KongrevskyDbContext : DbContextWithTriggers
    {
        public KongrevskyDbContext()
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider)
                : base(serviceProvider)
        {
            Configure();
        }

        public KongrevskyDbContext(DbCompiledModel model)
                : base(model)
        {
            Configure();
        }

        public KongrevskyDbContext(string nameOrConnectionString)
                : base(nameOrConnectionString)
        {
            Configure();
        }

        public KongrevskyDbContext(DbConnection existingConnection, bool contextOwnsConnection)
                : base(existingConnection, contextOwnsConnection)
        {
            Configure();
        }

        public KongrevskyDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
                : base(objectContext, dbContextOwnsObjectContext)
        {
            Configure();
        }

        public KongrevskyDbContext(string nameOrConnectionString, DbCompiledModel model)
                : base(nameOrConnectionString, model)
        {
            Configure();
        }

        public KongrevskyDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
                : base(existingConnection, model, contextOwnsConnection)
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider, DbCompiledModel model)
                : base(serviceProvider, model)
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider, string nameOrConnectionString)
                : base(serviceProvider, nameOrConnectionString)
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider, DbConnection existingConnection, bool contextOwnsConnection)
                : base(serviceProvider, existingConnection, contextOwnsConnection)
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider, ObjectContext objectContext, bool dbContextOwnsObjectContext)
                : base(serviceProvider, objectContext, dbContextOwnsObjectContext)
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider, string nameOrConnectionString, DbCompiledModel model)
                : base(serviceProvider, nameOrConnectionString, model)
        {
            Configure();
        }

        public KongrevskyDbContext(IServiceProvider serviceProvider, DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
                : base(serviceProvider, existingConnection, model, contextOwnsConnection)
        {
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