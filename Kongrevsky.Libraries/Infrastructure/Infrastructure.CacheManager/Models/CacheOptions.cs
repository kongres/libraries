namespace Kongrevsky.Infrastructure.CacheManager.Models
{
    public class CacheOptions
    {
        #region Properties

        public bool Enabled { get; set; }

        public int CachingTimeSeconds { get; set; }

        #endregion
    }
}