using System.Reflection;
using System.Runtime.Loader;
using Jellyfin.Plugin.PluginPages.Library;
using Jellyfin.Plugin.PluginPages.Manager;
using Jellyfin.Plugin.PluginPages.Services;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages
{
    public class PluginPagesServiceRegistrator : PluginServiceRegistrator
    {
        public override void ConfigureServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            serviceCollection.AddSingleton<IPluginPagesManager, PluginPagesManager>();
        }
    }
}