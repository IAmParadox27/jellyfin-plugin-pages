using Jellyfin.Plugin.PluginPages.Library;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages
{
    public class PluginPagesPlugin : BasePlugin<BasePluginConfiguration>
    {
        public override Guid Id => Guid.Parse("5b6550fa-a014-4f4c-8a2c-59a43680ac6d"); 
    
        public override string Name => "Plugin Pages";

        public static PluginPagesPlugin Instance { get; set; } = null!;
        internal IServerConfigurationManager ServerConfigurationManager { get; set; }
        
        public PluginPagesPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<PluginPagesPlugin> logger,
            IPluginPagesManager pluginPagesManager, IServerConfigurationManager serverConfigurationManager) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            
            ServerConfigurationManager = serverConfigurationManager;
            
            string configLocation = Path.Combine(applicationPaths.PluginConfigurationsPath, typeof(PluginPagesPlugin).Namespace!);
                  
            logger.LogInformation($"Loading plugin pages from {configLocation}");
        
            // Read the config and see if any have been defined in here.
            if (File.Exists(Path.Combine(configLocation, "config.json")))
            {
                logger.LogInformation($"Found config.json in {configLocation}");
                JObject config = JObject.Parse(File.ReadAllText(Path.Combine(configLocation, "config.json")));

                PluginPage[]? pages = JsonConvert.DeserializeObject<PluginPage[]>(config.Value<JArray>("pages")?.ToString() ?? "[]");

                if (pages != null)
                {
                    foreach (PluginPage? page in pages)
                    {
                        if (page != null)
                        {
                            logger.LogInformation($"Registering page: {page.Id} {page.DisplayText} {page.Url}");

                            pluginPagesManager.RegisterPluginPage(page);
                        }
                    }
                }
            }
        }
    }
}