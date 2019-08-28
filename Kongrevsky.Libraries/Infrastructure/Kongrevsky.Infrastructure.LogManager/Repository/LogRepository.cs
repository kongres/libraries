namespace Kongrevsky.Infrastructure.LogManager.Repository
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Kongrevsky.Infrastructure.LogManager.Infrastructure;
    using Kongrevsky.Infrastructure.LogManager.Models;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Infrastructure.Repository;
    using Kongrevsky.Infrastructure.Repository.Utils;
    using Kongrevsky.Utilities.Enumerable;
    using Kongrevsky.Utilities.String;
    using Microsoft.Extensions.Options;

    #endregion

    internal class LogRepository : KongrevskyRepository<LogItem, LogDbContext>, ILogRepository
    {
        private const string LogNotFound = "Log not found";

        public LogRepository(IKongrevskyDatabaseFactory<LogDbContext> kongrevskyDatabaseFactory,
                IOptions<LogManagerOptions> options)
                : base(kongrevskyDatabaseFactory)
        {
            _options = options.Value;

            if (_options.AppName.IsNullOrWhiteSpace())
                throw new ArgumentException("Config is required", nameof(_options.AppName));
        }

        private LogManagerOptions _options { get; }

        public async Task<ResultObjectInfo<LogItemDto>> GetLogAsync(string logId)
        {
            var log = await GetAsync(x => x.Id == logId && x.AppName == _options.AppName);
            if (log == null)
                return NotFound<LogItemDto>(LogNotFound);

            var mapper = AutoMapperDomainUtils.GetMapper(config => config.CreateMap<LogItem, LogItemDto>());

            return Ok(mapper.Map<LogItemDto>(log));
        }

        public async Task<ResultObjectInfo<LogItemDto>> GetLogAsync(int logNumber)
        {
            var log = await GetAsync(x => x.Number == logNumber && x.AppName == _options.AppName);
            if (log == null)
                return NotFound<LogItemDto>(LogNotFound);

            var mapper = AutoMapperDomainUtils.GetMapper(config => config.CreateMap<LogItem, LogItemDto>());

            return Ok(mapper.Map<LogItemDto>(log));
        }

        public async Task<LogItemPaging> GetLogsAsync(LogItemPaging filter)
        {
            var search = filter.Search.SplitBySpaces();

            var startDate = filter.StartDate?.ToUniversalTime();
            var endDate = filter.EndDate?.ToUniversalTime();

            var where = new List<Expression<Func<LogItem, bool>>>();
            where.AddIfTrue(startDate.HasValue, x => startDate <= x.DateCreated);
            where.AddIfTrue(endDate.HasValue, x => x.DateCreated <= endDate);
            where.AddIfTrue(filter.LogType.HasValue, x => filter.LogType == x.LogType);
            where.AddIfTrue(search.Any(), x => search.All(s => x.Action.Contains(s)));

            foreach (var filterAdditionalInfoLogItem in filter.AdditionalInfoLogItems)
            {
                switch (filterAdditionalInfoLogItem.Type)
                {
                    case AdditionalInfoLogItemFilterType.Contains:
                        where.Add(x => x.AdditionalInfoLogItems.FirstOrDefault(i => i.Name == filterAdditionalInfoLogItem.Name).Value.Contains(filterAdditionalInfoLogItem.Value));
                        break;
                    case AdditionalInfoLogItemFilterType.Equal:
                        where.Add(x => x.AdditionalInfoLogItems.FirstOrDefault(i => i.Name == filterAdditionalInfoLogItem.Name).Value == filterAdditionalInfoLogItem.Value);
                        break;
                    case AdditionalInfoLogItemFilterType.StartWith:
                        where.Add(x => x.AdditionalInfoLogItems.FirstOrDefault(i => i.Name == filterAdditionalInfoLogItem.Name).Value.StartsWith(filterAdditionalInfoLogItem.Value));
                        break;
                    case AdditionalInfoLogItemFilterType.EndWith:
                        where.Add(x => x.AdditionalInfoLogItems.FirstOrDefault(i => i.Name == filterAdditionalInfoLogItem.Name).Value.EndsWith(filterAdditionalInfoLogItem.Value));
                        break;
                }
            }

            Action<IMapperConfigurationExpression> configurationProvider = config => { config.CreateMap<LogItem, LogItemDto>(); };
            Expression<Func<LogItem, bool>> checkPermission = item => item.AppName == _options.AppName;

            var request = GetPage(filter, checkPermission, where, configurationProvider);

            await filter.SetResult(request);

            return filter;
        }

        public Task<string> CreateLogAsync(CreateLogItemDto log)
        {
            var newLog = new LogItem
                         {
                                 AppName = _options.AppName,
                                 LogType = log.LogType,
                                 Action = log.Action,
                                 UserEmail = log.UserEmail,
                                 UserName = log.UserName
                         };
            newLog.AdditionalInfoLogItems = log.AdditionalInfo.Select(x => new AdditionalInfoLogItem()
                                                                             {
                                                                                     LogItemId = newLog.Id,
                                                                                     Name = x.Name,
                                                                                     Value = x.Value
                                                                             }).ToList();

            Add(newLog);
            return Task.FromResult(newLog.Id);
        }

        public async Task<int> DeleteLogsAsync(DeleteLogsFilterDto filter)
        {
            var startDate = filter.StartDate?.ToUniversalTime();
            var endDate = filter.EndDate?.ToUniversalTime();

            var where = new List<Expression<Func<LogItem, bool>>>();
            where.Add(x => x.AppName == _options.AppName);
            where.AddIfTrue(startDate.HasValue, x => startDate <= x.DateCreated);
            where.AddIfTrue(endDate.HasValue, x => x.DateCreated <= endDate);
            where.AddIfTrue(filter.LogType.HasValue, x => filter.LogType == x.LogType);

            var queryable = GetMany(x => true);
            queryable = where.Aggregate(queryable, (current, expression) => current.Where(expression));

            var deletedNumber = BulkDelete(await queryable.ToListAsync(), item => item.Id);
            return deletedNumber;
        }

        public async Task<ResultInfo> DeleteLogAsync(string logId)
        {
            var log = await GetAsync(x => x.Id == logId && x.AppName == _options.AppName);
            if (log == null)
                return NotFound(LogNotFound);

            Delete(log);
            return Ok();
        }

        public async Task<ResultInfo> DeleteLogAsync(int logNumber)
        {
            var log = await GetAsync(x => x.Number == logNumber && x.AppName == _options.AppName);
            if (log == null)
                return NotFound(LogNotFound);

            Delete(log);
            return Ok();
        }
    }
}