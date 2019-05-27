namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.Configuration;
    using AutoMapper.QueryableExtensions;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Kongrevsky.Infrastructure.Repository.Triggers;
    using Kongrevsky.Infrastructure.Repository.Triggers.Bulk;
    using Kongrevsky.Infrastructure.Repository.Utils;
    using Kongrevsky.Infrastructure.Repository.Utils.Options;
    using Kongrevsky.Utilities.EF6.Models;
    using Kongrevsky.Utilities.Enumerable.Models;
    using Kongrevsky.Utilities.Object;
    using Kongrevsky.Utilities.String;
    using LinqKit;
    using Microsoft.Linq.Translations;
    using Z.EntityFramework.Plus;
    using QueryableUtils = Kongrevsky.Infrastructure.Repository.Utils.QueryableUtils;

    #endregion

    public class KongrevskyRepository<T, DB> : Repository, IKongrevskyRepository<T, DB>
            where T : class
            where DB : KongrevskyDbContext
    {
        public KongrevskyRepository(IKongrevskyDatabaseFactory<DB> kongrevskyDatabaseFactory)
        {
            _kongrevskyDatabaseFactory = kongrevskyDatabaseFactory;
        }

        private string _connectionString => DataContext.ConnectionString;

        private IKongrevskyDatabaseFactory<DB> _kongrevskyDatabaseFactory { get; }

        private DB _dataContext { get; set; }

        protected DbSet<T> DbsetRaw => DataContext.Set<T>();

        protected IQueryable<T> Dbset => DataContext.Set<T>().AsExpandable().WithTranslations();

        protected DB DataContext => _dataContext ?? (_dataContext = _kongrevskyDatabaseFactory.Get());

        private string connectionString => this._connectionString.IsNullOrEmpty() ? DataContext.Database.Connection.ConnectionString : this._connectionString;

        public virtual int BulkInsert(List<T> entities, Expression<Func<T, object>> identificator, Action<BulkInsertOptions<T>> configAction = null)
        {
            if (!entities.Any())
                return 0;

            var num = DataContext.BulkInsert(entities, identificator, configAction);
            return num;
        }

        public virtual int ClassicBulkInsert(List<T> entities)
        {
            DataContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var entity in entities)
                DataContext.Entry(entity).State = EntityState.Added;

            DataContext.Configuration.AutoDetectChangesEnabled = true;

            return entities.Count;
        }

        public virtual int BulkUpdate(List<T> entities, Expression<Func<T, object>> identificator, Action<BulkUpdateOptions<T>> configAction = null)
        {
            if (!entities.Any())
                return 0;

            var num = DataContext.BulkUpdate(entities, identificator, configAction);
            return num;
        }

        public virtual int ClassicBulkUpdate(List<T> entities)
        {
            DataContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var entity in entities)
                DataContext.Entry(entity).State = EntityState.Modified;

            DataContext.Configuration.AutoDetectChangesEnabled = true;

            return entities.Count;
        }

        public virtual int BulkDelete(List<T> entities, Expression<Func<T, object>> identificator, Action<BulkDeleteOptions<T>> configAction = null)
        {
            if (!entities.Any())
                return 0;

            var num = DataContext.BulkDelete(entities, identificator, configAction);
            return num;
        }

        public virtual int BulkDelete(Expression<Func<T, bool>> where, bool fireTriggers = true)
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

        public virtual int BulkDeleteDuplicates<Ts>(Expression<Func<T, Ts>> expression, Expression<Func<T, bool>> where = null, int batchSize = 5000, int bulkCopyTimeout = 600)
        {
            var num = 0;

            if (!typeof(BaseEntity).IsAssignableFrom(typeof(T)))
                return num;

            var records = new List<IGrouping<Ts, T>>();

            try
            {
                records.AddRange((where == null ? Dbset : Dbset.Where(where).WithTranslations()).GroupBy(expression).Where(x => x.Count() > 1).ToList());
            }
            catch (Exception e)
            {
                records.AddRange((where == null ? Dbset : Dbset.Where(where).WithTranslations()).ToList().GroupBy(expression.Compile()).Where(x => x.Count() > 1).ToList());
            }

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
                num = DataContext.BulkDelete(ent, x => (x as BaseEntity).Id,
                                                 config =>
                                                 {
                                                     config.BatchSize = batchSize;
                                                     config.Timeout = bulkCopyTimeout;
                                                 });
                return num;
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

        public virtual T Add(T entity)
        {
            DbsetRaw.Add(entity);
            return entity;
        }

        public virtual T Update(T entity)
        {
            DbsetRaw.AddOrUpdate(entity);
            return entity;
        }

        public virtual T AddOrUpdate(T entity)
        {
            DbsetRaw.AddOrUpdate(entity);
            return entity;
        }

        public virtual void AddOrUpdate(params T[] entities)
        {
            DbsetRaw.AddOrUpdate(entities);
        }

        public virtual T AddOrUpdate(Expression<Func<T, object>> identifierExpression, T entity)
        {
            DbsetRaw.AddOrUpdate(identifierExpression, entity);
            return entity;
        }

        public virtual void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities)
        {
            DbsetRaw.AddOrUpdate(identifierExpression, entities);
        }

        public virtual void Delete(T entity)
        {
            DbsetRaw.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var objects = Dbset.Where(where).WithTranslations().AsEnumerable();
            DbsetRaw.RemoveRange(objects);
        }

        public virtual T GetById(long id)
        {
            var entity = DbsetRaw.Find(id);
            if (entity != null)
                DataContext.Entry(entity).Reload();
            return entity;
        }

        public virtual T GetById(string id)
        {
            var entity = DbsetRaw.Find(id);
            if (entity != null)
                DataContext.Entry(entity).Reload();
            return entity;
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var entity = await DbsetRaw.FindAsync(id);
            if (entity != null)
                await DataContext.Entry(entity).ReloadAsync();
            return entity;
        }

        public virtual T GetById(Guid id)
        {
            var entity = DbsetRaw.Find(id);
            if (entity != null)
                DataContext.Entry(entity).Reload();
            return entity;
        }

        public T Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where).WithTranslations();
            query = AppendIncludes(query, includes);

            return query.FirstOrDefault();
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where).WithTranslations();
            query = AppendIncludes(query, includes);

            return query.FirstOrDefaultAsync();
        }

        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset;
            query = AppendIncludes(query, includes);

            return query;
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = Dbset.Where(where).WithTranslations();
            query = AppendIncludes(query, includes);

            return query;
        }

        public virtual PagingQueryable<TCast> GetPage<TCast>(RepositoryPagingModel<TCast> filter, Expression<Func<T, bool>> checkPermission, List<Expression<Func<T, bool>>> @where, Action<IMapperConfigurationExpression> configure, List<Expression<Func<TCast, bool>>> postWhere = null) where TCast : class
        {
            var page = new Page(filter.PageNumber, filter.PageSize);

            var queryable = Dbset;
            if (checkPermission != null)
                queryable = queryable.Where(checkPermission);

            foreach (var expression in where)
                queryable = queryable.Where(expression);

            var configurationProvider = AutoMapperDomainUtils.GetConfigurationProvider(config =>
                                                                                       {
                                                                                           configure.Invoke(config);
                                                                                           var mappingExpression = ((IProfileConfiguration)config).TypeMapConfigs.FirstOrDefault(x => x is IMappingExpression<T, TCast>) as IMappingExpression<T, TCast>;
                                                                                           mappingExpression?.LoadProperties(filter, DataContext);
                                                                                       });
            var castQuery = queryable.ProjectTo<TCast>(configurationProvider).AsExpandable().WithTranslations();

            if (postWhere != null)
                foreach (var expression in postWhere)
                    castQuery = castQuery.Where(expression);

            if (filter.Filters?.Any() ?? false)
                castQuery = castQuery.Where(QueryableUtils.FiltersToLambda<TCast>(filter.Filters));

            var orderProperties = filter.OrderProperty?.Split(new[] { ',', ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            IOrderedQueryable<TCast> orderedQuery;

            if (filter.Distinct.IsNullOrEmpty())
            {
                orderProperties.AddRange(_dataContext.GetKeyNames<T>());
                orderProperties = orderProperties.Distinct(new GenericCompare<string>(x => x.ToLowerInvariant())).ToList();

                if (orderProperties.Any())
                {
                    orderedQuery = filter.IsDesc ? castQuery.OrderByDescendingWithNullLowPriority(orderProperties.First()) : castQuery.OrderByWithNullLowPriority(orderProperties.First());
                    orderProperties.RemoveAt(0);
                }
                else
                {
                    orderedQuery = filter.IsDesc ? castQuery.OrderByDescendingWithNullLowPriority() : castQuery.OrderByWithNullLowPriority();
                }
                if (orderProperties.Any())
                    orderedQuery = orderProperties.Aggregate(orderedQuery, (current, property) => filter.IsDesc ? current.ThenByDescendingWithNullLowPriority(property) : current.ThenByWithNullLowPriority(property));
                else
                    orderedQuery = filter.IsDesc ? orderedQuery.ThenByDescendingWithNullLowPriority() : orderedQuery.ThenByWithNullLowPriority();
            }
            else
            {
                castQuery = castQuery.DistinctByField(filter.Distinct).AsExpandable();
                orderedQuery = filter.IsDesc ? castQuery.OrderByDescendingWithNullLowPriority(filter.Distinct) : castQuery.OrderByWithNullLowPriority(filter.Distinct);
            }

            return new PagingQueryable<TCast>(orderedQuery, page);
        }

        protected IQueryable<TSet> GetDbSet<TSet>(bool isExpanded = true) where TSet : class => isExpanded ? DataContext.Set<TSet>().AsExpandable().WithTranslations() : DataContext.Set<TSet>();

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