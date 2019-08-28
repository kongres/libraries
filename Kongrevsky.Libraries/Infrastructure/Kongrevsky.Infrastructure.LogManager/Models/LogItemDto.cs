namespace Kongrevsky.Infrastructure.LogManager.Models
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;

    #endregion

    public class LogItemDto
    {
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }

        public int Number { get; set; }

        public string AppName { get; set; }

        public LogItemType LogType { get; set; }

        public string Action { get; set; }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public virtual List<AdditionalInfoLogItemDto> AdditionalInfoLogItems { get; set; }
    }
}