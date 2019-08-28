namespace Kongrevsky.Infrastructure.LogManager.Models
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using Kongrevsky.Infrastructure.Repository.Models;

    #endregion

    public class LogItemPaging : RepositoryPagingModel<LogItemDto>
    {
        public LogItemPaging()
        {
            AdditionalInfoLogItems = new List<AdditionalInfoLogItemFilter>();
        }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public LogItemType? LogType { get; set; }

        public List<AdditionalInfoLogItemFilter> AdditionalInfoLogItems { get; set; }
    }

    public class AdditionalInfoLogItemFilter
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public AdditionalInfoLogItemFilterType Type { get; set; } = AdditionalInfoLogItemFilterType.Contains;
    }

    public enum AdditionalInfoLogItemFilterType
    {
        Contains = 0,

        Equal = 1,

        StartWith = 2,

        EndWith = 3,
    }
}