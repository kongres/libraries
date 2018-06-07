namespace Kongrevsky.Infrastructure.Repository.Attributes
{
    #region << Using >>

    using System;

    #endregion

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CaseSensitiveAttribute : Attribute
    {
        public CaseSensitiveAttribute()
        {
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }
    }
}