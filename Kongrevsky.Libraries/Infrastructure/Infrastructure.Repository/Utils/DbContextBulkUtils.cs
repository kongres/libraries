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
    using System.Threading.Tasks;
    using System.Transactions;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Repository.Triggers;
    using Kongrevsky.Infrastructure.Repository.Utils.Options;
    using Kongrevsky.Utilities.EF6;
    using Kongrevsky.Utilities.Object;
    using Nito.AsyncEx;
    using SqlBulkTools;
    using SqlBulkTools.Enumeration;

    #endregion

    public static class DbContextBulkUtils
    {
        private static readonly AsyncLock _lockObj = new AsyncLock();

        /// <summary>
        ///     Bulk Insert Entities in DB
        /// </summary>
        /// <typeparam name="DB">DB Context Type</typeparam>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="dbContext">DB Context</param>
        /// <param name="entities">List of Entities</param>
        /// <param name="identificator">
        ///     Expression which generate unique value for each entity. It can be one column or combination
        ///     of columns
        /// </param>
        /// <param name="configAction">Configure Bulk Insert Options</param>
        /// <returns></returns>
        public static int BulkInsert<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, Action<BulkInsertOptions<T>> configAction = null)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            var options = new BulkInsertOptions<T>();
            configAction?.Invoke(options);

            lock (dbContext)
            {
                if (options.FireTriggers)
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
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(options.Timeout)))
                {
                    var bulkInsert = new BulkOperations().Setup<T>()
                            .ForCollection(ent)
                            .WithTable(string.IsNullOrEmpty(options.TableName) ? dbContext.GetTableName<T>() : options.TableName)
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = options.BatchSize, BulkCopyTimeout = options.Timeout, SqlBulkCopyOptions = options.SqlBulkCopyOptions })
                            .AddAllColumns()
                            .DetectColumnWithCustomColumnName()
                            .RemoveNotMappedColumns()
                            .BulkInsert();

                    if (options.IdentityColumn != null)
                        bulkInsert = bulkInsert.SetIdentityColumn(options.IdentityColumn, ColumnDirectionType.Input);

                    num = bulkInsert.Commit(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseInserted(enumerable, dbContext, identificator);
                }

                return num;
            }
        }  
        
        /// <summary>
        ///     Bulk Insert Entities in DB
        /// </summary>
        /// <typeparam name="DB">DB Context Type</typeparam>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="dbContext">DB Context</param>
        /// <param name="entities">List of Entities</param>
        /// <param name="identificator">
        ///     Expression which generate unique value for each entity. It can be one column or combination
        ///     of columns
        /// </param>
        /// <param name="configAction">Configure Bulk Insert Options</param>
        /// <returns></returns>
        public static async Task<int> BulkInsertAsync<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, Action<BulkInsertOptions<T>> configAction = null)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            var options = new BulkInsertOptions<T>();
            configAction?.Invoke(options);

            using (await _lockObj.LockAsync())
            {
                if (options.FireTriggers)
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
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(options.Timeout)))
                {
                    var bulkInsert = new BulkOperations().Setup<T>()
                            .ForCollection(ent)
                            .WithTable(string.IsNullOrEmpty(options.TableName) ? dbContext.GetTableName<T>() : options.TableName)
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = options.BatchSize, BulkCopyTimeout = options.Timeout, SqlBulkCopyOptions = options.SqlBulkCopyOptions })
                            .AddAllColumns()
                            .DetectColumnWithCustomColumnName()
                            .RemoveNotMappedColumns()
                            .BulkInsert();

                    if (options.IdentityColumn != null)
                        bulkInsert = bulkInsert.SetIdentityColumn(options.IdentityColumn, ColumnDirectionType.Input);

                    num = await bulkInsert.CommitAsync(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseInserted(enumerable, dbContext, identificator);
                }

                return num;
            }
        }

        /// <summary>
        ///     Bulk Update Entities in DB
        /// </summary>
        /// <typeparam name="DB">DB Context Type</typeparam>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="dbContext">DB Context</param>
        /// <param name="entities">List of Entities</param>
        /// <param name="identificator">
        ///     Expression which generate unique value for each entity. It can be one column or combination
        ///     of columns
        /// </param>
        /// <param name="configAction">Configure Bulk Update Options</param>
        /// <returns></returns>
        public static int BulkUpdate<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, Action<BulkUpdateOptions<T>> configAction = null)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            var options = new BulkUpdateOptions<T>();
            configAction?.Invoke(options);

            lock (dbContext)
            {
                if (options.FireTriggers)
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

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(options.Timeout)))
                {
                    var bulkUpdate = new BulkOperations().Setup<T>()
                            .ForCollection(ent)
                            .WithTable(string.IsNullOrEmpty(options.TableName) ? dbContext.GetTableName<T>() : options.TableName)
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = options.BatchSize, BulkCopyTimeout = options.Timeout, SqlBulkCopyOptions = options.SqlBulkCopyOptions })
                            .AddAllColumns()
                            .DetectColumnWithCustomColumnName()
                            .RemoveNotMappedColumns()
                            .BulkUpdate();

                    num = bulkUpdate
                            .MatchTargetOn(identificator)
                            .Commit(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseUpdated(enumerable, dbContext, identificator);
                }

                return num;
            }
        }      
        
        /// <summary>
        ///     Bulk Update Entities in DB
        /// </summary>
        /// <typeparam name="DB">DB Context Type</typeparam>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="dbContext">DB Context</param>
        /// <param name="entities">List of Entities</param>
        /// <param name="identificator">
        ///     Expression which generate unique value for each entity. It can be one column or combination
        ///     of columns
        /// </param>
        /// <param name="configAction">Configure Bulk Update Options</param>
        /// <returns></returns>
        public static async Task<int> BulkUpdateAsync<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, Action<BulkUpdateOptions<T>> configAction = null)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            var options = new BulkUpdateOptions<T>();
            configAction?.Invoke(options);

            using (await _lockObj.LockAsync())
            {
                if (options.FireTriggers)
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

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(options.Timeout)))
                {
                    var bulkUpdate = new BulkOperations().Setup<T>()
                            .ForCollection(ent)
                            .WithTable(string.IsNullOrEmpty(options.TableName) ? dbContext.GetTableName<T>() : options.TableName)
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = options.BatchSize, BulkCopyTimeout = options.Timeout, SqlBulkCopyOptions = options.SqlBulkCopyOptions })
                            .AddAllColumns()
                            .DetectColumnWithCustomColumnName()
                            .RemoveNotMappedColumns()
                            .BulkUpdate();

                    num = await bulkUpdate
                            .MatchTargetOn(identificator)
                            .CommitAsync(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseUpdated(enumerable, dbContext, identificator);
                }

                return num;
            }
        }

        /// <summary>
        ///     Bulk Delete Entities in DB
        /// </summary>
        /// <typeparam name="DB">DB Context Type</typeparam>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="dbContext">DB Context</param>
        /// <param name="entities">List of Entities</param>
        /// <param name="identificator">
        ///     Expression which generate unique value for each entity. It can be one column or combination
        ///     of columns
        /// </param>
        /// <param name="configAction">Configure Bulk Delete Options</param>
        /// <returns></returns>
        public static int BulkDelete<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, Action<BulkDeleteOptions<T>> configAction = null)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            var options = new BulkDeleteOptions<T>();
            configAction?.Invoke(options);

            lock (dbContext)
            {
                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleting(enumerable, dbContext, identificator);
                }

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(options.Timeout)))
                {
                    var bulkDelete = new BulkOperations().Setup<T>()
                            .ForCollection(enumerable)
                            .WithTable(dbContext.GetTableName<T>())
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = options.BatchSize, BulkCopyTimeout = options.Timeout, SqlBulkCopyOptions = options.SqlBulkCopyOptions })
                            .AddColumn(identificator)
                            .BulkDelete();
                    num = bulkDelete
                            .MatchTargetOn(identificator)
                            .Commit(dbContext.Database.Connection as SqlConnection);

                    trans.Complete();
                }

                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleted(enumerable, dbContext);
                }

                return num;
            }
        }
        
        /// <summary>
        ///     Bulk Delete Entities in DB
        /// </summary>
        /// <typeparam name="DB">DB Context Type</typeparam>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="dbContext">DB Context</param>
        /// <param name="entities">List of Entities</param>
        /// <param name="identificator">
        ///     Expression which generate unique value for each entity. It can be one column or combination
        ///     of columns
        /// </param>
        /// <param name="configAction">Configure Bulk Delete Options</param>
        /// <returns></returns>
        public static async Task<int> BulkDeleteAsync<DB, T>(this DB dbContext, IEnumerable<T> entities, Expression<Func<T, object>> identificator, Action<BulkDeleteOptions<T>> configAction = null)
                where DB : DbContext
                where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any())
                return 0;

            var options = new BulkDeleteOptions<T>();
            configAction?.Invoke(options);

            using (await _lockObj.LockAsync())
            {
                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleting(enumerable, dbContext, identificator);
                }

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(options.Timeout)))
                {
                    var bulkDelete = new BulkOperations().Setup<T>()
                            .ForCollection(enumerable)
                            .WithTable(dbContext.GetTableName<T>())
                            .WithBulkCopySettings(new BulkCopySettings() { BatchSize = options.BatchSize, BulkCopyTimeout = options.Timeout, SqlBulkCopyOptions = options.SqlBulkCopyOptions })
                            .AddColumn(identificator)
                            .BulkDelete();

                    num = await bulkDelete
                            .MatchTargetOn(identificator)
                            .CommitAsync(dbContext.Database.Connection as SqlConnection);
                    
                    trans.Complete();
                }

                if (options.FireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleted(enumerable, dbContext);
                }

                return num;
            }
        }
    }
}