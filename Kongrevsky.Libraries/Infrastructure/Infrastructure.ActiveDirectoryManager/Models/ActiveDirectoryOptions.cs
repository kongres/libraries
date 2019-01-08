namespace Kongrevsky.Infrastructure.ActiveDirectoryManager.Models
{
    using System;
    using System.DirectoryServices.Protocols;

    public class ActiveDirectoryOptions
    {
        public string Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string AuthType { get; set; }

        public AuthType GetAuthType() => !string.IsNullOrEmpty(AuthType) && Enum.TryParse(AuthType, true, out AuthType parseAuthType) ? parseAuthType : System.DirectoryServices.Protocols.AuthType.Negotiate;
    }
}