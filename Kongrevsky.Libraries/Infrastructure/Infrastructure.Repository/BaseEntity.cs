namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System;
    using System.ComponentModel.DataAnnotations;
    using EntityFramework.Triggers;

    #endregion

    public class BaseEntity : BaseEntityWithoutKey
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        [Key]
        public string Id { get; set; }

        /// <summary>
        ///     Set new Id for the entity
        /// </summary>
        /// <param name="id"></param>
        public void SetId(string id)
        {
            Id = id;
        }

        /// <summary>
        ///     Generate new Id for the entity
        /// </summary>
        public void ResetId()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }

    public class BaseEntityWithoutKey
    {
        static BaseEntityWithoutKey()
        {
            Triggers<BaseEntity>.Inserting += entry => entry.Entity.DateCreated = entry.Entity.DateModified = DateTime.UtcNow;
            Triggers<BaseEntity>.Updating += entry => entry.Entity.DateModified = DateTime.UtcNow;
        }

        public BaseEntityWithoutKey()
        {
            DateCreated = DateModified = DateTime.UtcNow;
        }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}