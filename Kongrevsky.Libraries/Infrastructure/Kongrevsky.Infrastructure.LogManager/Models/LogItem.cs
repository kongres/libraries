namespace Kongrevsky.Infrastructure.LogManager.Models
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Net;
    using Kongrevsky.Infrastructure.Repository;

    #endregion

    public class LogItem : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; }

        public string AppName { get; set; }

        public LogItemType LogType { get; set; }

        public string Action { get; set; }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public virtual ICollection<AdditionalInfoLogItem> AdditionalInfoLogItems { get; set; }
    }
}