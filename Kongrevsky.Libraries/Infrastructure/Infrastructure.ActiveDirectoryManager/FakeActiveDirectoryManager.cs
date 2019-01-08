namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;
    using Kongrevsky.Utilities.String;
    using Microsoft.Extensions.Options;

    #endregion

    public class FakeActiveDirectoryManager : IActiveDirectoryManager
    {
        #region Properties

        private ActiveDirectoryOptions _activeDirectoryOptions { get; }

        private IEnumerable<ADUser> adUsers => new List<ADUser>()
                                               {
                                                       new ADUser() { Email = "test_admin@transperfect.com", Username = "test_admin", FirstName = "Admin", LastName = "System", IsActive = true },
                                                       new ADUser() { Email = "test_carol@transperfect.com", Username = "test_carol", FirstName = "Carol", LastName = "Rice", IsActive = true },
                                                       new ADUser() { Email = "test_cristina@transperfect.com", Username = "test_cristina", FirstName = "Cristina", LastName = "Obrien", IsActive = true },
                                                       new ADUser() { Email = "test_debra@transperfect.com", Username = "test_debra", FirstName = "Debra", LastName = "Tate", IsActive = true },
                                                       new ADUser() { Email = "test_arlene@transperfect.com", Username = "test_arlene", FirstName = "Arlene", LastName = "Curry", IsActive = false },
                                                       new ADUser() { Email = "test_erika@transperfect.com", Username = "test_erika", FirstName = "Erika", LastName = "Higgins", IsActive = true },
                                                       new ADUser() { Email = "test_leo@transperfect.com", Username = "test_leo", FirstName = "Leo", LastName = "Burke", IsActive = false },
                                                       new ADUser() { Email = "test_louis@transperfect.com", Username = "test_louis", FirstName = "Louis", LastName = "Henderson", IsActive = true },
                                                       new ADUser() { Email = "test_shawna@transperfect.com", Username = "test_shawna", FirstName = "Shawna", LastName = "Montgomery", IsActive = true },
                                                       new ADUser() { Email = "test_lynette@transperfect.com", Username = "test_lynette", FirstName = "Lynette", LastName = "Miller", IsActive = true },
                                                       new ADUser() { Email = "test_israel@transperfect.com", Username = "test_israel", FirstName = "Israel", LastName = "Hopkins", IsActive = true },
                                                       new ADUser() { Email = "test_perkins@transperfect.com", Username = "test_perkins", FirstName = "Austin", LastName = "Perkins", IsActive = true },
                                                       new ADUser() { Email = "test_padilla@transperfect.com", Username = "test_padilla", FirstName = "Everett", LastName = "Padilla", IsActive = true },
                                                       new ADUser() { Email = "test_darnell@transperfect.com", Username = "test_darnell", FirstName = "Darnell", LastName = "Wood", IsActive = true },
                                                       new ADUser() { Email = "test_marcos@transperfect.com", Username = "test_marcos", FirstName = "Marcos", LastName = "Simpson", IsActive = true }
                                               };

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