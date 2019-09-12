namespace Kongrevsky.Infrastructure.HelpManager.HelpDesk
{
    #region << Using >>

    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.Models;

    #endregion

    public interface IHelpDeskManager
    {
        Task<ResultInfo> AddRequestAsync(CreateHelpRequest model);
    }
}