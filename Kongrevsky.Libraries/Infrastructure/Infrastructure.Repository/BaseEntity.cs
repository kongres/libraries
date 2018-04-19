namespace Infrastructure.Repository
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EntityFramework.Triggers;

    public class BaseEntity : BaseEntityWithoutKey
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public void SetId(string id)
        {
            Id = id;
        }

        [Key]
        public string Id { get; private set; }
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