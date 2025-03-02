using System.IO.Pipes;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Jellyfin.Plugin.PluginPages.Helpers;
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
        private readonly NamedPipeService m_namedPipeService;

        public StartupService(IServerApplicationHost serverApplicationHost, ILogger<PluginPagesPlugin> logger, NamedPipeService namedPipeService)
        {
            m_serverApplicationHost = serverApplicationHost;
            m_logger = logger;
            m_namedPipeService = namedPipeService;
        }

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            List<JObject> payloads = new List<JObject>();

            {
                JObject payload = new JObject();
                payload.Add("id", "9340b171-0ae4-4d13-9970-9c4c4feba227");
                payload.Add("fileNamePattern", "index.html");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/IndexHtml");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.IndexHtml");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.IndexHtml", TransformationPatches.IndexHtml);

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "a837996a-2f73-4ad2-9467-e0410222d36f");
                payload.Add("fileNamePattern", "userpluginsettings.html");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/UserPlugin/SettingsHtml");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.SettingsHtml");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.SettingsHtml", TransformationPatches.SettingsHtml);

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "345a80b2-2ac2-42f5-834c-809947778a64");
                payload.Add("fileNamePattern", "user-plugin.undefined.chunk.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/UserPlugin/Javascript");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.UserPlugin.Javascript");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.UserPlugin.Javascript", TransformationPatches.UserPluginJs);

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "47a40f72-1af9-47b9-a2eb-e073317be4b0");
                payload.Add("fileNamePattern", "user-plugin-index-html.undefined.chunk.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/UserPlugin/IndexHtmlChunk");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.UserPlugin.IndexHtmlChunk");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.UserPlugin.IndexHtmlChunk", TransformationPatches.UserPluginIndexHtml);

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "898bd8c0-70fa-4f63-a4e8-bf16de51dfe4");
                payload.Add("fileNamePattern", "main.jellyfin.bundle.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/MainBundle/PluginSettingsRoute");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.MainBundle.PluginSettingsRoute");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.MainBundle.PluginSettingsRoute", TransformationPatches.MainBundlePluginSettingsRoute);

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "6e4bced9-d332-4b95-93f3-b82f088f56ab");
                payload.Add("fileNamePattern", "main.jellyfin.bundle.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/MainBundle/RouteIds");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.MainBundle.RouteIds");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.MainBundle.RouteIds", TransformationPatches.MainBundleRouteIds);

                payloads.Add(payload);
            }
            
            {
                JObject payload = new JObject();
                payload.Add("id", "a3530fb2-6d2f-4894-8ca7-691c4e98d1ed");
                payload.Add("fileNamePattern", "runtime.bundle.js");
                payload.Add("transformationEndpoint", "/PluginPages/Patch/RuntimeBundle");
                payload.Add("transformationPipe", "Jellyfin.Plugin.MediaBar.Patch.RuntimeBundle");
                RegisterPipeEndpoint("Jellyfin.Plugin.MediaBar.Patch.RuntimeBundle", TransformationPatches.RuntimeBundle);

                payloads.Add(payload);
            }
            
            
            string fileTransformationPipeName = "Jellyfin.Plugin.FileTransformation.NamedPipe";
            MethodInfo? getPipePathMethod = typeof(PipeStream).GetMethod("GetPipePath", BindingFlags.Static | BindingFlags.NonPublic);
            string? pipePath = getPipePathMethod?.Invoke(null, new object[] { ".", fileTransformationPipeName }) as string;
            string? pipeDirectory = Path.GetDirectoryName(pipePath);
            
            if (Directory.Exists(pipeDirectory) && Directory.GetFiles(pipeDirectory).Contains(pipePath))
            {
                foreach (JObject payload in payloads)
                {
                    NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", fileTransformationPipeName, PipeDirection.InOut);
                    await pipeClient.ConnectAsync();
                    byte[] payloadBytes = Encoding.UTF8.GetBytes(payload.ToString(Formatting.None));
                            
                    await pipeClient.WriteAsync(BitConverter.GetBytes((long)payloadBytes.Length));
                    await pipeClient.WriteAsync(payloadBytes, 0, payloadBytes.Length);
                            
                    pipeClient.ReadByte();
                    
                    await pipeClient.DisposeAsync();
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                
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
        }

        private void RegisterPipeEndpoint(string pipeName, Func<PatchRequestPayload, string> handler)
        {
            m_namedPipeService.CreateNamedPipeHandler(pipeName, async stream =>
            {
                m_logger.LogInformation($"Pipe Handler for: {pipeName}");
                byte[] lengthBuffer = new byte[8];
                await stream.ReadExactlyAsync(lengthBuffer, 0, lengthBuffer.Length);
                long length = BitConverter.ToInt64(lengthBuffer, 0);
                        
                MemoryStream memoryStream = new MemoryStream();
                while (length > 0)
                {
                    byte[] buffer = new byte[Math.Min(length, 1024)];
                    int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    length -= readBytes;
                            
                    memoryStream.Write(buffer, 0, readBytes);
                }
                        
                string rawJson = Encoding.UTF8.GetString(memoryStream.ToArray());
                
                string response = handler(JsonConvert.DeserializeObject<PatchRequestPayload>(rawJson)!);
                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                byte[] responseLengthBuffer = BitConverter.GetBytes((long)responseBuffer.Length);
                        
                await stream.WriteAsync(responseLengthBuffer, 0, responseLengthBuffer.Length);
                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            });
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