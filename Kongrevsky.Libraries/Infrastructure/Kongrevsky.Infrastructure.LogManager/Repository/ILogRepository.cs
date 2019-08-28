namespace Kongrevsky.Infrastructure.LogManager.Repository
{
    #region << Using >>

    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.LogManager.Models;
    using Kongrevsky.Infrastructure.Models;

    #endregion

    internal interface ILogRepository
    {
        Task<ResultObjectInfo<LogItemDto>> GetLogAsync(string logId);

        Task<ResultObjectInfo<LogItemDto>> GetLogAsync(int logNumber);

        Task<LogItemPaging> GetLogsAsync(LogItemPaging filter);

        Task<string> CreateLogAsync(CreateLogItemDto log);

        Task<int> DeleteLogsAsync(DeleteLogsFilterDto model);
    }
}