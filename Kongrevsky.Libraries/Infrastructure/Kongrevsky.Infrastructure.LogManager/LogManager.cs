namespace Kongrevsky.Infrastructure.LogManager
{
    #region << Using >>

    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.LogManager.Infrastructure;
    using Kongrevsky.Infrastructure.LogManager.Models;
    using Kongrevsky.Infrastructure.LogManager.Repository;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Infrastructure.Repository;

    #endregion

    internal class LogManager : ILogManager
    {
        public LogManager(ILogRepository logRepository, IKongrevskyUnitOfWork<LogDbContext> unitOfWork)
        {
            _logRepository = logRepository;
            _unitOfWork = unitOfWork;
        }

        private ILogRepository _logRepository { get; }

        private IKongrevskyUnitOfWork<LogDbContext> _unitOfWork { get; }

        public Task<ResultObjectInfo<LogItemDto>> GetLogAsync(string logId)
        {
            return _logRepository.GetLogAsync(logId);
        }

        public Task<ResultObjectInfo<LogItemDto>> GetLogAsync(int logNumber)
        {
            return _logRepository.GetLogAsync(logNumber);
        }

        public Task<LogItemPaging> GetLogsAsync(LogItemPaging filter)
        {
            return _logRepository.GetLogsAsync(filter);
        }

        public async Task<string> CreateLogAsync(CreateLogItemDto log)
        {
            var id = await _logRepository.CreateLogAsync(log);
            await _unitOfWork.CommitAsync();
            return id;
        }

        public Task<int> DeleteLogsAsync(DeleteLogsFilterDto filter)
        {
            return _logRepository.DeleteLogsAsync(filter);
        }
    }
}