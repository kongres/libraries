namespace Kongrevsky.Infrastructure.FileManager.Models
{
    public class FileManagerOptions
    {
        public string RootPath { get; set; }

        public ImpersonationOptions Impersonation { get; set; } = new ImpersonationOptions();
    }

    public class ImpersonationOptions
    {
        public bool IsImpersonationEnabled { get; set; } = false;

        public string ImpersonationDomain { get; set; }

        public string ImpersonationUsername { get; set; }

        public string ImpersonationPassword { get; set; }
    }
}