namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;

    #endregion

    public interface IActiveDirectoryManager
    {
        /// <summary>
        /// Detects if connection is alive
        /// </summary>
        /// <returns></returns>
        bool CheckConnection();

        /// <summary>
        /// Async returns an user by specified searching term
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<ADUser> FindUserAsync(string username);

        /// <summary>
        /// Async returns an user by specified email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<ADUser> GetUserByEmailAsync(string email);

        /// <summary>
        /// Async returns an user by specified username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<ADUser> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Async returns users by specified searching term
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<List<ADUser>> GetUsersAsync(string search);

        /// <summary>
        /// Async detects if specified credentials are valid
        /// </summary>
        /// <param name="login">Username or Email</param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> ValidateLoginAndPasswordAsync(string login, string password);        
        
        /// <summary>
        /// Async detects if specified credentials are valid
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> ValidateEmailAndPasswordAsync(string email, string password);        
        
        /// <summary>
        /// Async detects if specified credentials are valid
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> ValidateUsernameAndPasswordAsync(string username, string password);
    }
}