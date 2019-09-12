namespace Kongrevsky.Infrastructure.HelpManager.Models
{
    #region << Using >>

    using Kongrevsky.Infrastructure.HelpManager.HelpDesk.Models;
    using Kongrevsky.Infrastructure.HelpManager.Slack.Models;

    #endregion

    public class HelpManagerOptions
    {
        public HelpManagerOptions()
        {
            SlackOptions = new SlackOptions();
            HelpDeskOptions = new HelpDeskOptions();
        }

        public string AppName { get; set; }

        public bool IsSlackEnabled { get; set; }

        public SlackOptions SlackOptions { get; set; }

        public bool IsHelpDeskEnabled { get; set; }

        public HelpDeskOptions HelpDeskOptions { get; set; }
    }
}