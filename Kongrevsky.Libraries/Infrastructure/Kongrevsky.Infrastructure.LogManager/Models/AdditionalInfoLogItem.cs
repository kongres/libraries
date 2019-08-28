namespace Kongrevsky.Infrastructure.LogManager.Models
{
    #region << Using >>

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion

    public class AdditionalInfoLogItem
    {
        [Required, Key, Column(Order = 1)] public string LogItemId { get; set; }

        public virtual LogItem LogItem { get; set; }

        [Required, Key, Column(Order = 2)] public string Name { get; set; }

        public string Value { get; set; }
    }
}