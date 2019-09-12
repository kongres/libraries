namespace Kongrevsky.Infrastructure.HelpManager.Slack
{
    #region << Using >>

    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using global::Slack.Webhooks;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.HelpManager.Slack.Models;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Utilities.Common;
    using Microsoft.Extensions.Options;

    #endregion

    class SlackManager : ISlackManager
    {
        public SlackManager(IOptions<SlackOptions> options)
        {
            _options = options.Value ?? new SlackOptions();

            if (!_options.Channel?.StartsWith("#") ?? false)
                _options.Channel = "#" + _options.Channel;
        }

        private SlackOptions _options { get; }

        public Task<ResultInfo> SendMessageAsync(CreateHelpRequest message, string appName = null)
        {
            return RetryUtils.DoAsync(async () =>
                                      {
                                          var text = new StringBuilder();
                                          if (!string.IsNullOrEmpty(appName))
                                              text.AppendLine($"*AppName:* {appName}");
                                          text.AppendLine($"*From:* {message.Email}");
                                          text.AppendLine($"*Subject:* {message.Subject}");
                                          text.AppendLine($"*Description:* {message.Description}");

                                          var slack = new SlackClient(_options.Url);
                                          var slackMessage = new SlackMessage()
                                                             {
                                                                     Channel = _options.Channel,
                                                                     Text = text.ToString(),
                                                                     Username = _options.Username,
                                                                     IconEmoji = Emoji.Angel
                                                             };
                                          return await slack.PostAsync(slackMessage) ? new ResultInfo(HttpStatusCode.OK) : new ResultInfo(HttpStatusCode.BadRequest, "Error in post message");
                                      },
                                      TimeSpan.FromMilliseconds(500),
                                      3,
                                      false,
                                      exception => { LoggerUtils.Log("SlackManager exception", exception, nameof(SlackManager)); });
        }
    }
}