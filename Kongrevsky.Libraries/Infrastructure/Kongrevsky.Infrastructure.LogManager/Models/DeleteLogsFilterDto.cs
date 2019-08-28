namespace Kongrevsky.Infrastructure.LogManager.Models
{
    #region << Using >>

    using System;

    #endregion

    public class DeleteLogsFilterDto
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public LogItemType? LogType { get; set; }
    }
}