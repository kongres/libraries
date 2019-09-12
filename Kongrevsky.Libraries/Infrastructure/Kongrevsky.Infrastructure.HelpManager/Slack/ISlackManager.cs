namespace Kongrevsky.Infrastructure.HelpManager.Slack
{
    #region << Using >>

    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.Models;

    #endregion

    public interface ISlackManager
    {
        Task<ResultInfo> SendMessageAsync(CreateHelpRequest message, string appName = null);
    }
}