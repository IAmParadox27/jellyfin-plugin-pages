using Jellyfin.Plugin.PluginPages.Library;
using Jellyfin.Plugin.PluginPages.Manager;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.PluginPages
{
    public class PluginPagesServiceRegistrator : IPluginServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            serviceCollection.AddSingleton<IPluginPagesManager, PluginPagesManager>();
        }
    }
}