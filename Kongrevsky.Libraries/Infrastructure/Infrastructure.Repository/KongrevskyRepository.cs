namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Transactions;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Kongrevsky.Infrastructure.Repository.Triggers;
    using Kongrevsky.Infrastructure.Repository.Utils;
    using Kongrevsky.Utilities.EF6;
    using Kongrevsky.Utilities.EF6.Models;
    using Kongrevsky.Utilities.Enumerable.Models;
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.Reflection;
    using Kongrevsky.Utilities.String;
    using LinqKit;
    using SqlBulkTools;
    using SqlBulkTools.Enumeration;
    using Z.EntityFramework.Plus;
    using QueryableUtils = Utils.QueryableUtils;

    #endregion

    public class KongrevskyRepository<T, DB> : Repository, IKongrevskyRepository<T, DB>
            where T : class
            where DB : KongrevskyDbContext
    {
        private static readonly object _lockObject = new object();

        public KongrevskyRepository(IKongrevskyDatabaseFactory<DB> kongrevskyDatabaseFactory)
        {
            _kongrevskyDatabaseFactory = kongrevskyDatabaseFactory;
        }

        private string _connectionString => DataContext.ConnectionString;

        private IKongrevskyDatabaseFactory<DB> _kongrevskyDatabaseFactory { get; }

        private DB _dataContext { get; set; }

        protected DbSet<T> Dbset => DataContext.Set<T>();

        protected DB DataContext => _dataContext ?? (_dataContext = _kongrevskyDatabaseFactory.Get());

        private string connectionString => this._connectionString.IsNullOrEmpty() ? DataContext.Database.Connection.ConnectionString : this._connectionString;

        public virtual int BulkInsert(List<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true, int batchSize = 5000, int bulkCopyTimeout = 600)
        {
            if (!entities.Any())
                return 0;

            lock (_lockObject)
            {
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseInserting(entities, DataContext);
                }

                var config = new MapperConfiguration(conf =>
                                                     {
                                                         conf.CreateMap<T, T>().MaxDepth(1).ForAllMembers(c =>
                                                                                                          {
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.CustomAttributes.Any(x => x.AttributeType == typeof(ComplexTypeAttribute)) ?? false)
                                                                                                                  return;
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.IsSimple() ?? false)
                                                                                                                  return;
                                                                                                              c.Ignore();
                                                                                                          });
                                                     });
                var mapper = config.CreateMapper();

                var distEnts = entities.Distinct(new GenericCompare<T>(identificator)).ToList();
                var ent = mapper.Map<List<T>>(distEnts);

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        num = new BulkOperations().Setup<T>()
                                .ForCollection(ent)
                                .WithTable(DataContext.GetTableName<T>())
                                .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                                .AddAllColumns()
                                .DetectColumnWithCustomColumnName()
                                .BulkInsert()
                                .SetIdentityColumn(identificator, ColumnDirectionType.Input)
                                .Commit(conn);
                    }

                    trans.Complete();
                }

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseInserted(entities, DataContext, identificator);
                }

                return num;
            }
        }

        public virtual int ClassicBulkInsert(List<T> entities)
        {
            DataContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var entity in entities)
                DataContext.Entry(entity).State = EntityState.Added;

            DataContext.Configuration.AutoDetectChangesEnabled = true;

            return entities.Count;
        }

        public virtual int BulkUpdate(List<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true, int batchSize = 5000, int bulkCopyTimeout = 600)
        {
            if (!entities.Any())
                return 0;

            lock (_lockObject)
            {
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseUpdating(entities, DataContext, identificator);
                }

                var config = new MapperConfiguration(conf =>
                                                     {
                                                         conf.CreateMap<T, T>().MaxDepth(1).ForAllMembers(c =>
                                                                                                          {
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.CustomAttributes.Any(x => x.AttributeType == typeof(ComplexTypeAttribute)) ?? false)
                                                                                                                  return;
                                                                                                              if ((c.DestinationMember as PropertyInfo)?.PropertyType.IsSimple() ?? false)
                                                                                                                  return;
                                                                                                              c.Ignore();
                                                                                                          });
                                                     });
                var mapper = config.CreateMapper();

                var distEnts = entities.Distinct(new GenericCompare<T>(identificator)).ToList();
                var ent = mapper.Map<List<T>>(distEnts);
                //ent.ForEach(x => x.DateModified = DateTime.UtcNow);

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        num = new BulkOperations().Setup<T>()
                                .ForCollection(ent)
                                .WithTable(DataContext.GetTableName<T>())
                                .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                                .AddAllColumns()
                                .BulkUpdate()
                                .MatchTargetOn(identificator)
                                .Commit(conn);
                    }

                    trans.Complete();
                }

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseUpdated(entities, DataContext, identificator);
                }

                return num;
            }
        }

        public virtual int ClassicBulkUpdate(List<T> entities)
        {
            DataContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var entity in entities)
                DataContext.Entry(entity).State = EntityState.Modified;

            DataContext.Configuration.AutoDetectChangesEnabled = true;

            return entities.Count;
        }

        public virtual int BulkDelete(List<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true, int batchSize = 5000, int bulkCopyTimeout = 600)
        {
            if (!entities.Any())
                return 0;

            lock (_lockObject)
            {
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleting(entities, DataContext, identificator);
                }

                int num;
                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        num = new BulkOperations().Setup<T>()
                                .ForCollection(entities)
                                .WithTable(DataContext.GetTableName<T>())
                                .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                                .AddColumn(identificator)
                                .BulkDelete()
                                .MatchTargetOn(identificator)
                                .Commit(conn);
                    }

                    trans.Complete();
                }

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleted(entities, DataContext);
                }

                return num;
            }
        }

        public virtual int BulkDelete(Expression<Func<T, bool>> where, bool fireTriggers = true)
        {
            lock (_lockObject)
            {
                var entities = DataContext.Set<T>().Where(where).ToList();
                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleting(entities, DataContext);
                }

                var bulkDelete = DataContext.Set<T>().Where(where).Delete();

                if (fireTriggers)
                {
                    TriggersBulk<T, DB>.RaiseDeleted(entities, DataContext);
                }

                return bulkDelete;
            }
        }

        public virtual int BulkDeleteDuplicates<Ts>(Expression<Func<T, Ts>> expression, Expression<Func<T, bool>> where = null, int batchSize = 5000, int bulkCopyTimeout = 600)
        {
            var num = 0;
            lock (_lockObject)
            {
                if (!typeof(BaseEntity).IsAssignableFrom(typeof(T)))
                    return num;

                var query = (where == null ? Dbset : Dbset.Where(where)).GroupBy(expression).Where(x => x.Count() > 1).OrderBy(x => x.Key);
                var pageSize = 1000;
                var pageCount = query.GetPageCount(pageSize);
                var records = new List<IGrouping<Ts, T>>();

                for (var i = 1; i <= pageCount; i++)
                    records.AddRange(query.GetPage(new Page(i, pageSize)));

                var ent = new List<T>();
                foreach (var group in records)
                {
                    var entities = group.OrderBy(x => (x as BaseEntity)?.DateCreated).Skip(1).ToList();
                    ent.AddRange(entities);
                }

                if (!ent.Any())
                    return num;

                try
                {
                    var bulk = new BulkOperations();

                    using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(120)))
                    {
                        using (var conn = new SqlConnection(connectionString))
                        {
                            num = bulk.Setup<T>()
                                    .ForCollection(ent)
                                    .WithTable(DataContext.GetTableName<T>())
                                    .WithBulkCopySettings(new BulkCopySettings() { BatchSize = batchSize, BulkCopyTimeout = bulkCopyTimeout })
                                    .AddColumn(x => (x as BaseEntity).Id)
                                    .BulkDelete()
                                    .MatchTargetOn(x => (x as BaseEntity).Id)
                                    .Commit(conn);
                        }

                        trans.Complete();
                        return num;
                    }
                }
                catch (Exception e)
                {
                    var entIds = ent.Select(x => (x as BaseEntity).Id).ToList();

                    var containsMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Count() == 2)?.MakeGenericMethod(typeof(string));
                    var parameterExpression = Expression.Parameter(typeof(T), "x");
                    var expr = Expression.Call(null, containsMethod, Expression.Constant(entIds), Expression.Property(parameterExpression, nameof(BaseEntity.Id)));
                    var lambda = Expression.Lambda<Func<T, bool>>(expr, parameterExpression);
                    num = Dbset.Where(lambda).Delete();
                    return num;
                }
            }
        }

        public virtual T Add(T entity)
        {
            Dbset.Add(entity);
            return entity;
        }

        public virtual T Update(T entity)
        {
            Dbset.AddOrUpdate(entity);
            return entity;
        }

        public virtual T AddOrUpdate(T entity)
        {
            Dbset.AddOrUpdate(entity);
            return entity;
        }

        public virtual void AddOrUpdate(params T[] entities)
        {
            Dbset.AddOrUpdate(entities);
        }

        public virtual T AddOrUpdate(Expression<Func<T, object>> identifierExpression, T entity)
        {
            Dbset.AddOrUpdate(identifierExpression, entity);
            return entity;
        }

        public virtual void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities)
        {
            Dbset.AddOrUpdate(identifierExpression, entities);
        }

        public virtual void Delete(T entity)
        {
            Dbset.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var objects = Dbset.Where(where).AsEnumerable();
            Dbset.RemoveRange(objects);
        }

        public virtual T GetById(long id)
        {
            var entity = Dbset.Find(id);
            if (entity != null)
                DataContext.Entry(entity).Reload();
            return entity;
        }

        public virtual T GetById(string id)
        {
            var entity = Dbset.Find(id);
            if (entity != null)
                DataContext.Entry(entity).Reload();
            return entity;
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var entity = await Dbset.FindAsync(id);
            if (entity != null)
                await DataContext.Entry(entity).ReloadAsync();
            return entity;
        }

        public virtual T GetById(Guid id)
        {
            var entity = Dbset.Find(id);
            if (entity != null)
                DataContext.Entry(entity).Reload();
            return entity;
        }

        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = AppendIncludes(Dbset, includes);

            return query;
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.AsExpandable().Where(where);
            query = AppendIncludes(query, includes);

            return query;
        }

        public virtual PagingQueryable<TCast> GetPage<TCast>(RepositoryPagingModel<TCast> filter, Expression<Func<T, bool>> checkPermission, List<Expression<Func<T, bool>>> @where, IConfigurationProvider configurationProvider, List<Expression<Func<TCast, bool>>> postWhere = null) where TCast : class
        {
            var page = new Page(filter.PageNumber, filter.PageSize);

            var queryable = Dbset.AsExpandable();
            if (checkPermission != null)
                queryable = queryable.Where(checkPermission);

            foreach (var expression in where)
                queryable = queryable.Where(expression);

            var castQuery = queryable.ProjectTo<TCast>(configurationProvider).AsExpandable();

            if (postWhere != null)
                foreach (var expression in postWhere)
                    castQuery = castQuery.Where(expression);

            if (filter.Filters?.Any() ?? false)
                castQuery = castQuery.Where(QueryableUtils.FiltersToLambda<TCast>(filter.Filters));

            var orderProperties = filter.OrderProperty?.Split(new[] { ',', ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            IOrderedQueryable<TCast> orderedQuery;

            var distinctProperty = typeof(TCast).GetPropertyByName(filter.Distinct);
            if (distinctProperty != null && distinctProperty.PropertyType.IsSimple())
            {
                castQuery = castQuery.DistinctByField(distinctProperty.Name).AsExpandable();
                orderedQuery = filter.IsDesc ? castQuery.OrderByDescendingWithNullLowPriority(distinctProperty.Name) : castQuery.OrderByWithNullLowPriority(distinctProperty.Name);
            }
            else
            {
                if (orderProperties.Any())
                {
                    orderProperties.AddRange(_dataContext.GetKeyNames<T>());
                    orderProperties = orderProperties.Distinct(new GenericCompare<string>(x => x.ToLowerInvariant())).ToList();

                    orderedQuery = filter.IsDesc ? castQuery.OrderByDescendingWithNullLowPriority(orderProperties.First()) : castQuery.OrderByWithNullLowPriority(orderProperties.First());
                    orderProperties.RemoveAt(0);
                    if (orderProperties.Any())
                        orderedQuery = orderProperties.Aggregate(orderedQuery, (current, property) => filter.IsDesc ? current.ThenByDescendingWithNullLowPriority(property) : current.ThenByWithNullLowPriority(property));
                    else
                        orderedQuery = filter.IsDesc ? orderedQuery.ThenByDescendingWithNullLowPriority() : orderedQuery.ThenByWithNullLowPriority();
                }
                else
                {
                    orderProperties.AddRange(_dataContext.GetKeyNames<T>());
                    orderProperties = orderProperties.Distinct(new GenericCompare<string>(x => x.ToLowerInvariant())).ToList();

                    orderedQuery = filter.IsDesc ? castQuery.OrderByDescendingWithNullLowPriority() : castQuery.OrderByWithNullLowPriority();
                    if (orderProperties.Any())
                        orderedQuery = orderProperties.Aggregate(orderedQuery, (current, property) => filter.IsDesc ? current.ThenByDescendingWithNullLowPriority(property) : current.ThenByWithNullLowPriority(property));
                    else
                        orderedQuery = filter.IsDesc ? orderedQuery.ThenByDescendingWithNullLowPriority() : orderedQuery.ThenByWithNullLowPriority();
                }
            }

            return new PagingQueryable<TCast>(orderedQuery, page);
        }

        public T Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where);
            query = AppendIncludes(query, includes);

            return query.FirstOrDefault();
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where);
            query = AppendIncludes(query, includes);

            return query.FirstOrDefaultAsync();
        }

        protected IQueryable<TSet> GetDbSet<TSet>() where TSet : class => DataContext.Set<TSet>().AsExpandable();

        private IQueryable<T> AppendIncludes(IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includes)
        {
            return includes.Aggregate(query, (current, inc) => current.Include(inc));
        }
    }

    public abstract class Repository
    {
        #region Responses

        protected static ResultInfo Ok()
        {
            return new ResultInfo(HttpStatusCode.OK);
        }

        protected static ResultObjectInfo<T> Ok<T>(T resultObject)
        {
            return new ResultObjectInfo<T>(HttpStatusCode.OK, resultObject);
        }

        protected static ResultInfo Forbidden(string message = null)
        {
            return new ResultInfo(HttpStatusCode.Forbidden, message);
        }

        protected static ResultObjectInfo<T> Forbidden<T>(string message = null)
        {
            return new ResultObjectInfo<T>(HttpStatusCode.Forbidden, message);
        }

        protected static ResultInfo BadRequest(string message)
        {
            return new ResultInfo(HttpStatusCode.BadRequest, message);
        }

        protected static ResultObjectInfo<T> BadRequest<T>(string message)
        {
            return new ResultObjectInfo<T>(HttpStatusCode.BadRequest, message);
        }

        protected static ResultInfo NotFound(string message)
        {
            return new ResultInfo(HttpStatusCode.NotFound, message);
        }

        protected static ResultObjectInfo<T> NotFound<T>(string message)
        {
            return new ResultObjectInfo<T>(HttpStatusCode.NotFound, message);
        }

        protected static ResultInfo HttpStatusCodeResult(HttpStatusCode code, string message)
        {
            return new ResultInfo(code, message);
        }

        protected static ResultObjectInfo<T> HttpStatusCodeResult<T>(HttpStatusCode code, string message)
        {
            return new ResultObjectInfo<T>(code, message);
        }

        #endregion
    }
}