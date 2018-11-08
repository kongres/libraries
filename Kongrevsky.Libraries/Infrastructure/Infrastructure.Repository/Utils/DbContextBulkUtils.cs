namespace Kongrevsky.Infrastructure.Repository.Utils
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Transactions;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Repository.Triggers;
    using Kongrevsky.Utilities.EF6;
    using Kongrevsky.Utilities.Object;
    using SqlBulkTools;
    using SqlBulkTools.Enumeration;

    #endregion

    public static class DbContextBulkUtils
    {
        public static int BulkInsert<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true, int batchSize = 5000, int bulkCopyTimeout = 600)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            lock (dbContext)
            {
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseInserting(enumerable, dbContext);
                }

                var config = new MapperConfiguration(conf =>
                                                     {
                                                         conf.CreateMap<T, T>().MaxDepth(1).ForAllMembers(c =>
                                                                                                          {
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.CustomAttributes.Any(x => x.AttributeType == typeof(NotMappedAttribute)) ?? false)
                                                                                                              {
                                                                                                                  c.Ignore();
                                                                                                              }

                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.CustomAttributes.Any(x => x.AttributeType == typeof(ComplexTypeAttribute)) ?? false)
                                                                                                                  return;
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.IsSimple() ?? false)
                                                                                                                  return;
                                                                                                              c.Ignore();
                                                                                                          });
                                                     });
                var mapper = config.CreateMapper();

                var distEnts = enumerable.Distinct(new GenericCompare<T>(identificator)).ToList();
                var ent = mapper.Map<List<T>>(distEnts);

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                {
                    num = new BulkOperations().Setup<T>()
                            .ForCollection(ent)
                            .WithTable(dbContext.GetTableName<T>())
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                            .AddAllColumns()
                            .DetectColumnWithCustomColumnName()
                            .RemoveNotMappedColumns()
                            .BulkInsert()
                            //.SetIdentityColumn(identificator, ColumnDirectionType.Input)
                            .Commit(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseInserted(enumerable, dbContext, identificator);
                }

                return num;
            }
        }

        public static int BulkUpdate<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true, int batchSize = 5000, int bulkCopyTimeout = 600)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            lock (dbContext)
            {
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseUpdating(enumerable, dbContext, identificator);
                }

                var config = new MapperConfiguration(conf =>
                                                     {
                                                         conf.CreateMap<T, T>().MaxDepth(1).ForAllMembers(c =>
                                                                                                          {
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.CustomAttributes.Any(x => x.AttributeType == typeof(NotMappedAttribute)) ?? false)
                                                                                                              {
                                                                                                                  c.Ignore();
                                                                                                              }

                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.CustomAttributes.Any(x => x.AttributeType == typeof(ComplexTypeAttribute)) ?? false)
                                                                                                                  return;
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.IsSimple() ?? false)
                                                                                                                  return;
                                                                                                              c.Ignore();
                                                                                                          });
                                                     });
                var mapper = config.CreateMapper();

                var distEnts = enumerable.Distinct(new GenericCompare<T>(identificator)).ToList();
                var ent = mapper.Map<List<T>>(distEnts);
                //ent.ForEach(x => x.DateModified = DateTime.UtcNow);

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                {
                    num = new BulkOperations().Setup<T>()
                            .ForCollection(ent)
                            .WithTable(dbContext.GetTableName<T>())
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                            .AddAllColumns()
                            .DetectColumnWithCustomColumnName()
                            .RemoveNotMappedColumns()
                            .BulkUpdate()
                            .MatchTargetOn(identificator)
                            .Commit(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseUpdated(enumerable, dbContext, identificator);
                }

                return num;
            }
        }

        public static int BulkDelete<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true, int batchSize = 5000, int bulkCopyTimeout = 600)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            lock (dbContext)
            {
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleting(enumerable, dbContext, identificator);
                }

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                {
                    num = new BulkOperations().Setup<T>()
                            .ForCollection(enumerable)
                            .WithTable(dbContext.GetTableName<T>())
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                            .AddColumn(identificator)
                            .BulkDelete()
                            .MatchTargetOn(identificator)
                            .Commit(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleted(enumerable, dbContext);
                }

                return num;
            }
        }
    }
}