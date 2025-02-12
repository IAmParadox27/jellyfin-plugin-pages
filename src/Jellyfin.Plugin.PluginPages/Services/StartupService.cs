using System.Net.Http.Headers;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages.Services
{
    public class StartupService : IScheduledTask
    {
        private readonly IServerApplicationHost m_serverApplicationHost;
        private readonly IApplicationPaths m_applicationPaths;
        private readonly ILogger<PluginPagesPlugin> m_logger;

        public StartupService(IServerApplicationHost serverApplicationHost, IApplicationPaths applicationPaths, ILogger<PluginPagesPlugin> logger)
        {
            m_serverApplicationHost = serverApplicationHost;
            m_applicationPaths = applicationPaths;
            m_logger = logger;
        }

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            List<JObject> payloads = new List<JObject>();

            {
                JObject payload = new JObject();
                payload.Add("id", "9340b171-0ae4-4d13-9970-9c4c4feba227");
                payload.Add("fileNamePattern", "index.html");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/IndexHtml");

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "a837996a-2f73-4ad2-9467-e0410222d36f");
                payload.Add("fileNamePattern", "userpluginsettings.html");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/UserPlugin/SettingsHtml");

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "345a80b2-2ac2-42f5-834c-809947778a64");
                payload.Add("fileNamePattern", "user-plugin.undefined.chunk.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/UserPlugin/Javascript");

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "47a40f72-1af9-47b9-a2eb-e073317be4b0");
                payload.Add("fileNamePattern", "user-plugin-index-html.undefined.chunk.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/UserPlugin/IndexHtmlChunk");

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "898bd8c0-70fa-4f63-a4e8-bf16de51dfe4");
                payload.Add("fileNamePattern", "main.jellyfin.bundle.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/MainBundle/PluginSettingsRoute");

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "6e4bced9-d332-4b95-93f3-b82f088f56ab");
                payload.Add("fileNamePattern", "main.jellyfin.bundle.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/MainBundle/RouteIds");

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "a3530fb2-6d2f-4894-8ca7-691c4e98d1ed");
                payload.Add("fileNamePattern", "runtime.bundle.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/RuntimeBundle");

                payloads.Add(payload);
            }
            
            
            string? publishedServerUrl = m_serverApplicationHost.GetType()
                .GetProperty("PublishedServerUrl", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(m_serverApplicationHost) as string;
            
            foreach (JObject payload in payloads)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(publishedServerUrl ?? $"http://localhost:{m_serverApplicationHost.HttpPort}");

                try
                {
                    await client.PostAsync("/FileTransformation/RegisterTransformation",
                        new StringContent(payload.ToString(Formatting.None),
                            MediaTypeHeaderValue.Parse("application/json")));
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, $"Caught exception when attempting to register file transformation. Ensure you have `File Transformation` plugin installed on your server.");
                    return;
                }
            }
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo()
            {
                Type = TaskTriggerInfo.TriggerStartup
            };
        }

        public string Name => "PluginPages Startup";

        public string Key => "Jellyfin.Plugin.PluginPages.Startup";
        
        public string Description => "Startup Service for PluginPages";
        
        public string Category => "Startup Services";
    }
}