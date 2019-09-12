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

            return (helpDeskResult?.IsSuccess ?? true) && (slackResult?.IsSuccess ?? true) ? new ResultInfo(HttpStatusCode.OK) : new ResultInfo(HttpStatusCode.BadRequest, "Creation Request Failed");
        }
    }
}