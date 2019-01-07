namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.Protocols;
    using System.Linq;
    using System.Net;
    using System.Security.Policy;
    using System.Threading.Tasks;
    using Kongrevsky.Infrastructure.ActiveDirectoryManager.Models;
    using Kongrevsky.Utilities.String;
    using Microsoft.Extensions.Options;

    #endregion

    public class ActiveDirectoryManager : IActiveDirectoryManager
    {
        private const int ERROR_LOGON_FAILURE = 0x31;

        #region Properties

        private ActiveDirectoryOptions _activeDirectoryOptions { get; }

        private DirectoryEntry _directoryEntry
        {
            get
            {
                var directoryEntry = new DirectoryEntry(_activeDirectoryOptions.Url, _activeDirectoryOptions.Username, _activeDirectoryOptions.Password);
                return directoryEntry;
            }
        }

        #endregion

        #region Constructors

        public ActiveDirectoryManager(IOptions<ActiveDirectoryOptions> activeDirectoryOptions)
        {
            _activeDirectoryOptions = activeDirectoryOptions.Value;
        }

        #endregion

        #region Interface Implementations

        public bool CheckConnection()
        {
            try
            {
                var checkConnection = ValidateUserCredentialsAsync(_activeDirectoryOptions.Username, _activeDirectoryOptions.Password).Result;
                return checkConnection;
            }
            catch
            {
                return false;
            }
        }

        public Task<ADUser> GetUserByEmailAsync(string email)
        {
            return Task.Run(() =>
                            {
                                var directorySearcher = new DirectorySearcher(_directoryEntry);
                                directorySearcher.Filter = $"(&(objectClass=user)(mail={email}))";
                                directorySearcher.PropertiesToLoad.Add("givenName");          // first name
                                directorySearcher.PropertiesToLoad.Add("sn");                 // last name
                                directorySearcher.PropertiesToLoad.Add("mail");               // smtp mail address
                                directorySearcher.PropertiesToLoad.Add("userAccountControl"); // state of user
                                directorySearcher.PropertiesToLoad.Add("telephoneNumber");    // phone
                                directorySearcher.PropertiesToLoad.Add("sAMAccountName");     // phone

                                var result = directorySearcher.FindOne();

                                return result == null ?
                                               null :
                                               new ADUser()
                                               {
                                                   Email = FirstOrDefault(result.Properties["mail"]) as string,
                                                   FirstName = FirstOrDefault(result.Properties["givenName"]) as string,
                                                   LastName = FirstOrDefault(result.Properties["sn"]) as string,
                                                   Phone = FirstOrDefault(result.Properties["telephoneNumber"]) as string,
                                                   Username = FirstOrDefault(result.Properties["sAMAccountName"]) as string,
                                                   IsActive = IsUserActive(result.GetDirectoryEntry())
                                               };
                            });
        }

        public Task<ADUser> GetUserByUsernameAsync(string username)
        {
            return Task.Run(() =>
                            {
                                var directorySearcher = new DirectorySearcher(_directoryEntry);
                                directorySearcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                                directorySearcher.PropertiesToLoad.Add("givenName");          // first name
                                directorySearcher.PropertiesToLoad.Add("sn");                 // last name
                                directorySearcher.PropertiesToLoad.Add("mail");               // smtp mail address
                                directorySearcher.PropertiesToLoad.Add("userAccountControl"); // state of user
                                directorySearcher.PropertiesToLoad.Add("telephoneNumber");    // phone
                                directorySearcher.PropertiesToLoad.Add("sAMAccountName");     // phone

                                var result = directorySearcher.FindOne();

                                return result == null ?
                                               null :
                                               new ADUser()
                                               {
                                                   Email = FirstOrDefault(result.Properties["mail"]) as string,
                                                   FirstName = FirstOrDefault(result.Properties["givenName"]) as string,
                                                   LastName = FirstOrDefault(result.Properties["sn"]) as string,
                                                   Phone = FirstOrDefault(result.Properties["telephoneNumber"]) as string,
                                                   Username = FirstOrDefault(result.Properties["sAMAccountName"]) as string,
                                                   IsActive = IsUserActive(result.GetDirectoryEntry())
                                               };
                            });
        }

        public Task<List<ADUser>> GetUsersAsync(string search)
        {
            return Task.Run(() =>
                            {
                                var searchList = search.SplitBySpaces();
                                searchList = searchList.Any() ? searchList.Select(x => $"*{x}*").ToList() : new List<string>() { "*" };

                                var searchAd = string.Join("", searchList.Select(x => $"(givenName={x})(sn={x})(mail={x})"));
                                var searcher = new DirectorySearcher(_directoryEntry)
                                {
                                    Filter = $"(&(objectClass=user)(|{searchAd}))"
                                };
                                //searcher.SizeLimit = sizeLimit;

                                searcher.PropertiesToLoad.Add("givenName");          // first name
                                searcher.PropertiesToLoad.Add("sn");                 // last name
                                searcher.PropertiesToLoad.Add("mail");               // smtp mail address
                                searcher.PropertiesToLoad.Add("userAccountControl"); // state of user
                                searcher.PropertiesToLoad.Add("telephoneNumber");    // phone
                                searcher.PropertiesToLoad.Add("sAMAccountName");     // phone
                                var resultCollection = searcher.FindAll();

                                var adUsers = new List<ADUser>();

                                foreach (SearchResult o in resultCollection)
                                {
                                    var adUser = new ADUser()
                                    {
                                        Email = FirstOrDefault(o.Properties["mail"]) as string,
                                        FirstName = FirstOrDefault(o.Properties["givenName"]) as string,
                                        LastName = FirstOrDefault(o.Properties["sn"]) as string,
                                        Phone = FirstOrDefault(o.Properties["telephoneNumber"]) as string,
                                        Username = FirstOrDefault(o.Properties["sAMAccountName"]) as string,
                                        IsActive = IsUserActive(o.GetDirectoryEntry())
                                    };

                                    if (!string.IsNullOrEmpty(adUser.Email))
                                        adUsers.Add(adUser);
                                }

                                return adUsers.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ThenBy(x => x.Email).ToList();
                            });
        }

        public Task<ADUser> FindUserAsync(string search)
        {
            return Task.Run(() =>
                            {
                                search = string.IsNullOrEmpty(search) ? "*" : $"*{search}*";
                                var directorySearcher = new DirectorySearcher(_directoryEntry);
                                directorySearcher.Filter = $"(&(objectClass=user)(|(givenName={search})(sn={search})(mail={search})))";
                                directorySearcher.PropertiesToLoad.Add("givenName");          // first name
                                directorySearcher.PropertiesToLoad.Add("sn");                 // last name
                                directorySearcher.PropertiesToLoad.Add("mail");               // smtp mail address
                                directorySearcher.PropertiesToLoad.Add("userAccountControl"); // state of user
                                directorySearcher.PropertiesToLoad.Add("telephoneNumber");    // phone
                                directorySearcher.PropertiesToLoad.Add("sAMAccountName");     // phone

                                var result = directorySearcher.FindOne();

                                return result == null ?
                                               null :
                                               new ADUser()
                                               {
                                                   Email = FirstOrDefault(result.Properties["mail"]).ToString(),
                                                   FirstName = FirstOrDefault(result.Properties["givenName"])?.ToString(),
                                                   LastName = FirstOrDefault(result.Properties["sn"])?.ToString(),
                                                   Phone = FirstOrDefault(result.Properties["telephoneNumber"])?.ToString(),
                                                   Username = FirstOrDefault(result.Properties["sAMAccountName"])?.ToString(),
                                                   IsActive = IsUserActive(result.GetDirectoryEntry())
                                               };
                            });
        }

        public Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            return Task.Run(() =>
                            {
                                try
                                {
                                    username = username?.Split('@').FirstOrDefault();
                                    var domain = _activeDirectoryOptions.Url;
                                    if (Uri.TryCreate(_activeDirectoryOptions.Url, UriKind.Absolute, out var uri))
                                    {
                                        domain = uri.Host;
                                    }

                                    var credentials = new NetworkCredential(username, password, domain);

                                    var directoryIdentifier = new LdapDirectoryIdentifier(domain);

                                    using (var connection = new LdapConnection(directoryIdentifier, credentials, AuthType.Negotiate))
                                    {
                                        connection.SessionOptions.Sealing = true;
                                        connection.SessionOptions.Signing = true;

                                        connection.Bind();
                                    }
                                    return true;
                                }
                                catch (LdapException lEx)
                                {
                                    if (ERROR_LOGON_FAILURE == lEx.ErrorCode)
                                    {
                                        return false;
                                    }
                                }

                                return false;
                            });
        }

        #endregion

        private bool IsUserActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null)
                return false;

            var flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        private object FirstOrDefault(ResultPropertyValueCollection collection)
        {
            if (collection == null)
                return null;

            if (collection.Count > 0)
                return collection?[0];

            return null;
        }
    }
}