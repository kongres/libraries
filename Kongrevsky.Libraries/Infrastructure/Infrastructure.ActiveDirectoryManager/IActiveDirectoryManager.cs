namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;

    #endregion

    public interface IActiveDirectoryManager
    {
        bool CheckConnection();

        Task<ADUser> FindUserAsync(string username);

        Task<ADUser> GetUserByEmailAsync(string email);

        Task<ADUser> GetUserByUsernameAsync(string username);

        Task<List<ADUser>> GetUsersAsync(string search);

        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}