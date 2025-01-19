using System.Reflection;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.PluginPages.Library;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages;

public class Plugin : BasePlugin<BasePluginConfiguration>
{
    public override Guid Id => Guid.Parse("5b6550fa-a014-4f4c-8a2c-59a43680ac6d"); 
    
    public override string Name => "Plugin Pages";
    
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger,
        IServerConfigurationManager configurationManager, IPluginPagesManager pluginPagesManager) : base(applicationPaths, xmlSerializer)
    {
        if (!string.IsNullOrWhiteSpace(applicationPaths.WebPath))
        {
            string indexFile = Path.Combine(applicationPaths.WebPath, "index.html");
            if (File.Exists(indexFile))
            {
                string indexContents = File.ReadAllText(indexFile);
                string basePath = "";

                // Get base path from network config
                try
                {
                    object networkConfig = configurationManager.GetConfiguration("network");
                    Type configType = networkConfig.GetType();
                    PropertyInfo? basePathField = configType.GetProperty("BaseUrl");
                    string? confBasePath = basePathField?.GetValue(networkConfig)?.ToString()?.Trim('/');

                    if (!string.IsNullOrEmpty(confBasePath)) basePath = "/" + confBasePath.ToString();
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to get base path from config, using '/': {0}", e);
                }

                // Don't run if script already exists
                string scriptReplace = "<script plugin=\"PluginPages\".*?></script>";
                string scriptElement = string.Format("<script plugin=\"PluginPages\" version=\"1.0.0.0\" src=\"{0}/PluginPages/inject.js\" defer></script>", basePath);

                if (!indexContents.Contains(scriptElement))
                {
                    logger.LogInformation("Attempting to inject plugin pages script code in {0}", indexFile);

                    // Replace old Jellyscrub scrips
                    indexContents = Regex.Replace(indexContents, scriptReplace, "");

                    // Insert script last in body
                    int bodyClosing = indexContents.LastIndexOf("</body>");
                    if (bodyClosing != -1)
                    {
                        indexContents = indexContents.Insert(bodyClosing, scriptElement);

                        try
                        {
                            File.WriteAllText(indexFile, indexContents);
                            logger.LogInformation("Finished injecting plugin pages script code in {0}", indexFile);
                        }
                        catch (Exception e)
                        {
                            logger.LogError("Encountered exception while writing to {0}: {1}", indexFile, e);
                        }
                    }
                    else
                    {
                        logger.LogInformation("Could not find closing body tag in {0}", indexFile);
                    }
                }
                else
                {
                    logger.LogInformation("Found client script injected in {0}", indexFile);
                }
            }
        }
        
        string configLocation = Path.Combine(applicationPaths.PluginConfigurationsPath, "Paradox.PluginPages");
                  
        logger.LogInformation($"Loading plugin pages from {configLocation}");
        // Read the config and see if any have been defined in here.
        if (File.Exists(Path.Combine(configLocation, "config.json")))
        {
            logger.LogInformation($"Found config.json in {configLocation}");
            JObject config = JObject.Parse(File.ReadAllText(Path.Combine(configLocation, "config.json")));

            PluginPage[] pages = JsonConvert.DeserializeObject<PluginPage[]>(config.Value<JArray>("pages").ToString());
            
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