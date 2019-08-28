namespace Kongrevsky.Infrastructure.LogManager.Models
{
    #region << Using >>

    using System.Collections.Generic;

    #endregion

    public class CreateLogItemDto
    {
        public CreateLogItemDto()
        {
            AdditionalInfo = new List<CreateAdditionalInfoLogItemDto>();
        }

        public LogItemType LogType { get; set; }

        public string Action { get; set; }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public List<CreateAdditionalInfoLogItemDto> AdditionalInfo { get; set; }
    }
}