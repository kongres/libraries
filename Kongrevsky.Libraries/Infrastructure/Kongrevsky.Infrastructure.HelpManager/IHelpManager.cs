namespace Kongrevsky.Infrastructure.HelpManager
{
    #region << Using >>

    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.Models;

    #endregion

    public interface IHelpManager
    {
        Task<ResultInfo> CreateRequestAsync(CreateHelpRequest request);
    }
}