namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Utils;
    using Kongrevsky.Utilities.String;
    using Microsoft.Extensions.Options;

    #endregion

    public class FakeActiveDirectoryManager : IActiveDirectoryManager
    {
        #region Properties

        private ActiveDirectoryOptions _activeDirectoryOptions { get; }

        private static IEnumerable<ADUser> adUsers { get; set; }

        public static void SeedAdUsers(IEnumerable<ADUser> users)
        {
            adUsers = users.ToList();
        }

        #endregion

        #region Constructors

        public FakeActiveDirectoryManager(IOptions<ActiveDirectoryOptions> activeDirectoryOptions)
        {
            _activeDirectoryOptions = activeDirectoryOptions.Value;
        }

        #endregion

        #region Interface Implementations

        public bool CheckConnection()
        {
            return true;
        }

        public Task<ADUser> FindUserAsync(string username)
        {
            return Task.FromResult(adUsers.FirstOrDefault(x => x.Email.Contains(username, StringComparison.InvariantCultureIgnoreCase) ||
                                                               x.FirstName.Contains(username, StringComparison.InvariantCultureIgnoreCase) ||
                                                               x.LastName.Contains(username, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<ADUser> GetUserByEmailAsync(string email)
        {
            return Task.FromResult(adUsers.FirstOrDefault(x => string.Equals(x.Email, email, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<ADUser> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(adUsers.FirstOrDefault(x => string.Equals(x.Username, username, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<List<ADUser>> GetUsersAsync(string search)
        {
            var searchList = search.SplitBySpaces().ToList();

            var users = adUsers.Where(x => !searchList.Any() || searchList.All(s => x.Email.Contains(s, StringComparison.InvariantCultureIgnoreCase) ||
                                                                                    x.FirstName.Contains(s, StringComparison.InvariantCultureIgnoreCase) ||
                                                                                    x.LastName.Contains(s, StringComparison.InvariantCultureIgnoreCase)))
                               .ToList();

            return Task.FromResult(users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ThenBy(x => x.Email).ToList());
        }

        public Task<bool> ValidateLoginAndPasswordAsync(string login, string password)
        {
            return ValidatorUtils.IsValidEmail(login) ? ValidateEmailAndPasswordAsync(login, password) : ValidateUsernameAndPasswordAsync(login, password);
        }

        public Task<bool> ValidateEmailAndPasswordAsync(string email, string password)
        {
            var username = email?.Split('@').FirstOrDefault();
            return ValidateUsernameAndPasswordAsync(username, password);
        }

        public Task<bool> ValidateUsernameAndPasswordAsync(string username, string password)
        {
            return Task.FromResult(adUsers.Any(x => string.Equals(x.Email, username, StringComparison.InvariantCultureIgnoreCase) || string.Equals(x.Username, username, StringComparison.InvariantCultureIgnoreCase)) && password == _activeDirectoryOptions.Password.IfNullOrEmpty("123456789"));
        }

        #endregion
    }
}