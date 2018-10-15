namespace Kongrevsky.Utilities.EF6
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Linq.Expressions;

    #endregion

    public static class DbContextUtils
    {
        /// <summary>
        ///     Removes collection of entities got by predicate from the context underlying the set with each entity being put into
        ///     the Deleted state such that it will be deleted from the database when SaveChanges is called.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set">An <see cref="T:System.Linq.IQueryable`1" /> to filter to delete.</param>
        /// <param name="where">A function to test each element for a condition</param>
        public static void RemoveRange<TEntity>(this DbSet<TEntity> set, Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            var entities = set.Where(where).ToList();
            set.RemoveRange(entities);
        }

        /// <summary>
        ///     Returns Table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            var entitySet = GetEntitySet<T>(context);
            if (entitySet == null)
                throw new Exception($"Unable to find entity set '{typeof(T).Name}' in edm metadata");

            //var tableName = GetStringProperty(entitySet, "Schema") + "." + GetStringProperty(entitySet, "Table");
            var tableName = GetStringProperty(entitySet, "Table");
            return tableName;
        }

        /// <summary>
        ///     Returns Entity by specified Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static object FindRecordById<T>(this T context, string id) where T : DbContext
        {
            var dbContextType = context.GetType();
            var sets = dbContextType.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(p => p.GetValue(context, null)).ToList();

            foreach (var set in sets)
            {
                var theMethod = set.GetType().GetMethod("Find");
                try
                {
                    var obj = theMethod.Invoke(set, new object[] { new object[] { id } });
                    if (obj != null)
                        return obj;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns Entity by specified EntityKey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        public static object FindRecordByEntityKey<T>(this T context, EntityKey entityKey) where T : DbContext
        {
            var sets = context.GetType().GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .ToList();

            var set = sets.FirstOrDefault(x => x.Name == entityKey.EntitySetName);

            if (set != null)
                try
                {
                    var dbSet = set.GetValue(context, null);
                    var queryable = dbSet as IQueryable;
                    var parameters = entityKey.EntityKeyValues.ToList();
                    var whereStr = string.Join(" and ", parameters.Select(x => $"{x.Key} == @{parameters.IndexOf(x)}"));
                    var obj = queryable.AsNoTracking().Where(whereStr, parameters.Select(x => x.Value).ToArray()).ToListAsync().Result.FirstOrDefault();

                    if (obj != null)
                        return obj;
                }
                catch (Exception e)
                {
                    // ignored
                }

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            return objectContext.GetObjectByKey(entityKey);
        }

        /// <summary>
        ///     Returns all added relationships
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<object, object>> GetAddedRelationships(this DbContext context)
        {
            return GetRelationships(context, EntityState.Added, (e, i) => e.CurrentValues[i]);
        }

        /// <summary>
        ///     Returns all deleted relationships
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<object, object>> GetDeletedRelationships(this DbContext context)
        {
            return GetRelationships(context, EntityState.Deleted, (e, i) => e.OriginalValues[i]);
        }

        /// <summary>
        /// Insert Entities using classic workflow with disabled <see cref="DbContextConfiguration.AutoDetectChangesEnabled"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int ClassicBulkInsert<T>(this DbContext dbContext, IEnumerable<T> entities) where T : class
        {
            dbContext.Configuration.AutoDetectChangesEnabled = false;
            foreach (T entity in entities)
                dbContext.Entry<T>(entity).State = EntityState.Added;
            dbContext.Configuration.AutoDetectChangesEnabled = true;
            return entities.Count();
        }

        private static IEnumerable<Tuple<object, object>> GetRelationships(DbContext context, EntityState relationshipState, Func<ObjectStateEntry, int, object> getValue)
        {
            context.ChangeTracker.DetectChanges();
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext
                    .ObjectStateManager
                    .GetObjectStateEntries(relationshipState)
                    .Where(e => e.IsRelationship)
                    .Select(
                            e => Tuple.Create(context.FindRecordByEntityKey((EntityKey)getValue(e, 0)), context.FindRecordByEntityKey((EntityKey)getValue(e, 1))));
        }

        private static string GetStringProperty(MetadataItem entitySet, string propertyName)
        {
            MetadataProperty property;
            if (entitySet == null)
                throw new ArgumentNullException(nameof(entitySet));

            if (entitySet.MetadataProperties.TryGetValue(propertyName, false, out property))
            {
                string str;
                if (property?.Value != null && (str = property.Value as string) != null && !string.IsNullOrEmpty(str))
                    return str;
            }

            return string.Empty;
        }

        private static EntitySet GetEntitySet<T>(DbContext context)
        {
            var type = typeof(T);
            var entityName = type.Name;
            var entityBaseName = type.BaseType?.Name;
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            var entitySets = metadata.GetItemCollection(DataSpace.SSpace)
                    .GetItems<EntityContainer>()
                    .Single()
                    .BaseEntitySets
                    .OfType<EntitySet>()
                    .Where(s => !s.MetadataProperties.Contains("Type") || s.MetadataProperties["Type"].ToString() == "Tables")
                    .ToList();

            var entitySet = entitySets.FirstOrDefault(t => t.Name == entityName) ?? entitySets.FirstOrDefault(t => t.Name == entityBaseName);

            return entitySet;
        }
    }
}