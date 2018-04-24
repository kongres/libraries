namespace Kongrevsky.Infrastructure.ActiveDirectoryManager
{
    #region << Using >>

    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class ActiveDirectoryExtensions
    {
        public static void AddKongrevskyActiveDirectoryManager(this IServiceCollection services, bool isFake)
        {
            if (isFake)
                services.AddTransient<IActiveDirectoryManager, FakeActiveDirectoryManager>();
            else
                services.AddTransient<IActiveDirectoryManager, ActiveDirectoryManager>();
        }
    }
}