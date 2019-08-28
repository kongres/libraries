namespace Kongrevsky.Infrastructure.LogManager
{
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.LogManager.Models;
    using Kongrevsky.Infrastructure.Models;

    public interface ILogManager
    {
        Task<ResultObjectInfo<LogItemDto>> GetLogAsync(string logId);

        Task<ResultObjectInfo<LogItemDto>> GetLogAsync(int logNumber);

        Task<LogItemPaging> GetLogsAsync(LogItemPaging filter);

        Task<string> CreateLogAsync(CreateLogItemDto log);

        Task<int> DeleteLogsAsync(DeleteLogsFilterDto filter);
    }
}