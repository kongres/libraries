namespace Kongrevsky.Infrastructure.ActiveDirectoryManager.Tests
{
    using Kongrevsky.Infrastructure.ActiveDirectoryManager;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class ActiveDirectoryManager_Tests
    {
        public ActiveDirectoryManager_Tests()
        {
            _activeDirectoryOptions = new ActiveDirectoryOptions()
                                      {
                                              Password = "123456",
                                              Url = "https://google.com",
                                              Username = "userName"
                                      };
            _activeDirectoryManager = new FakeActiveDirectoryManager(new OptionsWrapper<ActiveDirectoryOptions>(_activeDirectoryOptions));
        }

        private ActiveDirectoryOptions _activeDirectoryOptions { get; }
        private IActiveDirectoryManager _activeDirectoryManager { get; }

        [Fact]
        public void ReturnUserGivenValueOfTestAdmin()
        {
            var result = _activeDirectoryManager.GetUserAsync("test_admin").Result;

            Assert.NotNull(result);
        }

        [Fact]
        public void ReturnNullGivenValueOfUnknown()
        {
            var result = _activeDirectoryManager.GetUserAsync("unknown").Result;

            Assert.Null(result);
        }

        [Fact]
        public void ReturnUserGivenExistingEmail()
        {
            var result = _activeDirectoryManager.GetUserByEmailAsync("test_admin@mailmail.com").Result;

            Assert.NotNull(result);
        }

        [Fact]
        public void ReturnNullGivenNotExistingEmail()
        {
            var result = _activeDirectoryManager.GetUserByEmailAsync("unknown@mail.com").Result;

            Assert.Null(result);
        }

        [Fact]
        public void ReturnUserGivenExistingUserName()
        {
            var result = _activeDirectoryManager.GetUserByUsernameAsync("test_admin").Result;

            Assert.NotNull(result);
        }

        [Fact]
        public void ReturnNullGivenNotExistingUserName()
        {
            var result = _activeDirectoryManager.GetUserByUsernameAsync("unknown").Result;

            Assert.Null(result);
        }

        [Fact]
        public void ReturnTrueGivenValidCredentials()
        {
            var result = _activeDirectoryManager.ValidateUsernameAndPasswordAsync("test_admin", _activeDirectoryOptions.Password).Result;

            Assert.True(result);
        }

        [Fact]
        public void ReturnFalseGivenNotValidCredentials()
        {
            var result = _activeDirectoryManager.ValidateUsernameAndPasswordAsync("unknown", "notexistingpassword)*&%$^%$").Result;

            Assert.False(result);
        }
    }
}