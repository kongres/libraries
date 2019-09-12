namespace Kongrevsky.Infrastructure.HelpManager
{
    #region << Using >>

    using System.Net;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.HelpManager.HelpDesk;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.HelpManager.Slack;
    using Kongrevsky.Infrastructure.Models;
    using Microsoft.Extensions.Options;

    #endregion

    public class HelpManager : IHelpManager
    {
        public HelpManager(IOptions<HelpManagerOptions> options, ISlackManager slackManager, IHelpDeskManager helpDeskManager)
        {
            _slackManager = slackManager;
            _helpDeskManager = helpDeskManager;
            _options = options.Value;
        }

        private HelpManagerOptions _options { get; }

        private ISlackManager _slackManager { get; }

        private IHelpDeskManager _helpDeskManager { get; }

        public async Task<ResultInfo> CreateRequestAsync(CreateHelpRequest request)
        {
            ResultInfo helpDeskResult = null;
            if (_options.IsHelpDeskEnabled)
                helpDeskResult = await _helpDeskManager.AddRequestAsync(request);

            ResultInfo slackResult = null;
            if (_options.IsSlackEnabled)
                slackResult = await _slackManager.SendMessageAsync(request, _options.AppName);

            if ((helpDeskResult?.IsSuccess ?? true) && (slackResult?.IsSuccess ?? true))
                return new ResultInfo(HttpStatusCode.OK);

            return helpDeskResult?.StatusCode == HttpStatusCode.GatewayTimeout ||
                   helpDeskResult?.StatusCode == HttpStatusCode.RequestTimeout ||
                   slackResult?.StatusCode == HttpStatusCode.RequestTimeout ||
                   slackResult?.StatusCode == HttpStatusCode.RequestTimeout
                    ? new ResultInfo(HttpStatusCode.BadRequest, "Help center service was not able to receive your request at this time. Please try again or contact administrator for more information.")
                    : new ResultInfo(HttpStatusCode.BadRequest, "Current Request can’t be submitted at this time. Please try again or contact administrator for more information.");
        }
    }
}