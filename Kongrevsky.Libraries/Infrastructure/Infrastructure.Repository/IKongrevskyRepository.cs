namespace Kongrevsky.Infrastructure.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Kongrevsky.Infrastructure.Repository.Models;
    using Kongrevsky.Utilities.Enumerable.Models;

    public interface IKongrevskyRepository<T, DB> : IKongrevskyRepository<T> 
        where T : class
        where DB : KongrevskyDbContext
    { }

    public interface IKongrevskyRepository<T> where T : class
    {
        int BulkInsert(List<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true);
        int ClassicBulkInsert(List<T> entities);

        int BulkUpdate(List<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true);
        int ClassicBulkUpdate(List<T> entities);

        int BulkDelete(List<T> entities, Expression<Func<T, object>> identificator, bool fireTriggers = true);
        int BulkDelete(Expression<Func<T, bool>> where, bool fireTriggers = true);
        int BulkDeleteDuplicates<Ts>(Expression<Func<T, Ts>> expression, Expression<Func<T, bool>> where = null);

        T Add(T entity);
        T Update(T entity);
        T AddOrUpdate(T entity);
        void AddOrUpdate(params T[] entities);
        T AddOrUpdate(Expression<Func<T, object>> identifierExpression, T entity);
        void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(long id);
        T GetById(string id);
        Task<T> GetByIdAsync(string id);
        T GetById(Guid id);
        T Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        Task<T> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetMany(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        PagingQueryable<TCast> GetPage<TCast>(PagingModel<TCast> filter, Expression<Func<T, bool>> checkPermission, List<Expression<Func<T, bool>>> where, IConfigurationProvider configurationProvider, List<Expression<Func<TCast, bool>>> postWhere = null) where TCast : class;
    }
}