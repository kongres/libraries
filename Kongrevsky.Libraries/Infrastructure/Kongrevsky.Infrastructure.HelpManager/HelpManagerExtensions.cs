namespace Kongrevsky.Infrastructure.HelpManager
{
    #region << Using >>

    using System;
    using Kongrevsky.Infrastructure.HelpManager.HelpDesk;
    using Kongrevsky.Infrastructure.HelpManager.HelpDesk.Models;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.HelpManager.Slack;
    using Kongrevsky.Infrastructure.HelpManager.Slack.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    #endregion

    public static class HelpManagerExtensions
    {
        public static void AddHelpManager(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddOptions();
            services.Configure<HelpManagerOptions>(configurationSection);
            services.Configure<HelpDeskOptions>(configurationSection.GetSection(nameof(HelpManagerOptions.HelpDeskOptions)));
            services.Configure<SlackOptions>(configurationSection.GetSection(nameof(HelpManagerOptions.SlackOptions)));
            services.AddTransient<ISlackManager, SlackManager>();
            services.AddTransient<IHelpDeskManager, HelpDeskManager>();
            services.AddTransient<IHelpManager, HelpManager>();
        }
        
        public static void AddHelpManager(this IServiceCollection services, Func<IConfigurationSection> configurationSection)
        {
            services.AddHelpManager(configurationSection());
        }

        public static void AddHelpManager(this IServiceCollection services, Action<HelpManagerOptions> configurationSection)
        {
            var options = new HelpManagerOptions();
            configurationSection(options);

            services.AddOptions();
            services.AddTransient<IOptions<HelpManagerOptions>>(s=> new OptionsWrapper<HelpManagerOptions>(options));
            services.AddTransient<IOptions<SlackOptions>>(s=> new OptionsWrapper<SlackOptions>(options.SlackOptions));
            services.AddTransient<IOptions<HelpDeskOptions>>(s=> new OptionsWrapper<HelpDeskOptions>(options.HelpDeskOptions));
            services.AddTransient<ISlackManager, SlackManager>();
            services.AddTransient<IHelpDeskManager, HelpDeskManager>();
            services.AddTransient<IHelpManager, HelpManager>();
        }
    }
}