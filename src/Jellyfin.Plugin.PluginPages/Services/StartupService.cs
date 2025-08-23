using System.IO.Pipes;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Jellyfin.Plugin.PluginPages.Helpers;
using Jellyfin.Plugin.PluginPages.JellyfinVersionSpecific;
using Jellyfin.Plugin.PluginPages.Model;
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
        private readonly ILogger<PluginPagesPlugin> m_logger;

        public StartupService(IServerApplicationHost serverApplicationHost, ILogger<PluginPagesPlugin> logger)
        {
            m_serverApplicationHost = serverApplicationHost;
            m_logger = logger;
        }

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            List<JObject> payloads = new List<JObject>();

            {
                JObject payload = new JObject();
                payload.Add("id", "9340b171-0ae4-4d13-9970-9c4c4feba227");
                payload.Add("fileNamePattern", "index.html");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.IndexHtml));

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "a837996a-2f73-4ad2-9467-e0410222d36f");
                payload.Add("fileNamePattern", "userpluginsettings.html");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.SettingsHtml));

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "345a80b2-2ac2-42f5-834c-809947778a64");
                payload.Add("fileNamePattern", "user-plugin.undefined.chunk.js");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.UserPluginJs));

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "47a40f72-1af9-47b9-a2eb-e073317be4b0");
                payload.Add("fileNamePattern", "user-plugin-index-html.undefined.chunk.js");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.UserPluginIndexHtml));

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "898bd8c0-70fa-4f63-a4e8-bf16de51dfe4");
                payload.Add("fileNamePattern", "main.jellyfin.bundle.js");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.MainBundlePluginSettingsRoute));

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "6e4bced9-d332-4b95-93f3-b82f088f56ab");
                payload.Add("fileNamePattern", "main.jellyfin.bundle.js");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.MainBundleRouteIds));

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "a3530fb2-6d2f-4894-8ca7-691c4e98d1ed");
                payload.Add("fileNamePattern", "runtime.bundle.js");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.RuntimeBundle));

                payloads.Add(payload);
            }
            
            Assembly? fileTransformationAssembly =
                AssemblyLoadContext.All.SelectMany(x => x.Assemblies).FirstOrDefault(x =>
                    x.FullName?.Contains(".FileTransformation") ?? false);

            if (fileTransformationAssembly != null)
            {
                Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");

                if (pluginInterfaceType != null)
                {
                    foreach (JObject payload in payloads)
                    {
                        pluginInterfaceType.GetMethod("RegisterTransformation")?.Invoke(null, new object?[] { payload });
                    }
                }
            }
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers() => StartupServiceHelper.GetDefaultTriggers();

        public string Name => "PluginPages Startup";

        public string Key => "Jellyfin.Plugin.PluginPages.Startup";
        
        public string Description => "Startup Service for PluginPages";
        
        public string Category => "Startup Services";
    }
}