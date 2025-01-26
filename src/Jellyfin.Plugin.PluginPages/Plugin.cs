using System.Collections;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.FileTransformation.Controller;
using Jellyfin.Plugin.FileTransformation.Infrastructure;
using Jellyfin.Plugin.PluginPages.Generated;
using Jellyfin.Plugin.PluginPages.Library;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages;

public class Plugin : BasePlugin<BasePluginConfiguration>
{
    public override Guid Id => Guid.Parse("5b6550fa-a014-4f4c-8a2c-59a43680ac6d"); 
    
    public override string Name => "Plugin Pages";

    private string[] UserPluginPagesIds = new[]
    {
        "'./controllers/user/plugin/index.js'",
        "'user-plugin'"
    };

    private string[] UserPluginPagesHtmlIds = new[]
    {
        "'./controllers/user/plugin/index.html'",
        "'user-plugin-index-html'"
    };
    
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger,
        IPluginPagesManager pluginPagesManager, IWebFileTransformationWriteService webFileTransformationWriteService) : base(applicationPaths, xmlSerializer)
    {
        webFileTransformationWriteService.AddTransformation("index.html", InjectIndexHtmlScript);
        webFileTransformationWriteService.AddTransformation("userpluginsettings.html", InjectUserPluginSettings);
        webFileTransformationWriteService.AddTransformation("user-plugin.undefined.chunk.js", InjectUserPluginSettingsJs);
        webFileTransformationWriteService.AddTransformation("user-plugin-index-html.undefined.chunk.js", InjectUserPluginSettingsHtml);
        webFileTransformationWriteService.AddTransformation("main.jellyfin.bundle.js", InjectUserPluginSettingsRoute);
        webFileTransformationWriteService.AddTransformation("main.jellyfin.bundle.js", InjectRouteIds);
        webFileTransformationWriteService.AddTransformation("runtime.bundle.js", InjectRuntimeBundle);
        
        string configLocation = Path.Combine(applicationPaths.PluginConfigurationsPath, typeof(Plugin).Namespace!);
                  
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

    private void InjectUserPluginSettingsJs(string path, Stream contents)
    {
        Stream? fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(Plugin).Namespace}.Controller.userpluginsettings.html");

        if (fileStream != null)
        {
            using var textReader = new StreamReader(fileStream);
            using var textWriter = new StreamWriter(contents, null, -1, true);
            textWriter.WriteLine("\"use strict\";");
            textWriter.Write($"(self.webpackChunk = self.webpackChunk || []).push([[{UserPluginPagesIds[1]}], {{{UserPluginPagesIds[0]}:function(a,e,t){{t.r(e),e.default = '");
            textWriter.Write(textReader.ReadToEnd().Replace("\r", "").Replace("\n", "").Replace("'", "\\'"));
            textWriter.Write("'}}]);");
        }
    }
    private void InjectUserPluginSettingsHtml(string path, Stream contents)
    {
        Stream? fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(Plugin).Namespace}.Controller.userpluginsettings.html");

        if (fileStream != null)
        {
            using var textReader = new StreamReader(fileStream);
            using var textWriter = new StreamWriter(contents, null, -1, true);
            textWriter.WriteLine("\"use strict\";");
            textWriter.Write($"(self.webpackChunk = self.webpackChunk || []).push([[{UserPluginPagesHtmlIds[1]}], {{{UserPluginPagesHtmlIds[0]}:function(a,e,t){{t.r(e),e.default = '");
            textWriter.Write(textReader.ReadToEnd().Replace("\r", "").Replace("\n", "").Replace("'", "\\'"));
            textWriter.Write("'}}]);");
        }
    }

    private void InjectRouteIds(string path, Stream contents)
    {
        using var textReader = new StreamReader(contents, null, true, -1, true);
        var text = textReader.ReadToEnd();

        string scriptElement = @$"""./user/plugin/index"":[{string.Join(',', UserPluginPagesIds)}],";
        scriptElement += @$"""./user/plugin/index.html"":[{string.Join(',', UserPluginPagesHtmlIds)}],";
        var regex = Regex.Replace(text, "(\"\\.\\/home\\.html\")", $"{scriptElement}$1");
        contents.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StreamWriter(contents, null, -1, true);
        textWriter.Write(regex);
    }

    private void InjectRuntimeBundle(string path, Stream contents)
    {
        using var textReader = new StreamReader(contents, null, true, -1, true);
        var text = textReader.ReadToEnd();
        
        string scriptElement = @$"{UserPluginPagesIds[1]}:""user-plugin"",";
        scriptElement += @$"{UserPluginPagesHtmlIds[1]}:""user-plugin-index-html"",";
        var regex = Regex.Replace(text, "(8372:\"home-html\")", $"{scriptElement}$1");
        contents.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StreamWriter(contents, null, -1, true);
        textWriter.Write(regex);
    }

    private void InjectUserPluginSettingsRoute(string path, Stream contents)
    {
        string scriptElement = @"path:""userpluginsettings.html"",pageProps:{controller:""user/plugin/index"",view:""user/plugin/index.html""}},{";

        using var textReader = new StreamReader(contents, null, true, -1, true);
        var text = textReader.ReadToEnd();
        var regex = Regex.Replace(text, "(path:\"queue\")", $"{scriptElement}$1");
        contents.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StreamWriter(contents, null, -1, true);
        textWriter.Write(regex);
    }

    private void InjectUserPluginSettings(string path, Stream contents)
    {
        Stream? fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(Plugin).Namespace}.Controller.userpluginsettings.html");

        if (fileStream != null)
        {
            using var textReader = new StreamReader(fileStream);
            using var textWriter = new StreamWriter(contents, null, -1, true);
            textWriter.Write(textReader.ReadToEnd());
        }
    }

    private void InjectIndexHtmlScript(string path, Stream contents)
    {
        string scriptElement = "<script plugin=\"PluginPages\" version=\"1.0.0.0\" src=\"/PluginPages/inject.js\" defer></script>";

        using var textReader = new StreamReader(contents, null, true, -1, true);
        var text = textReader.ReadToEnd();
        var regex = Regex.Replace(text, "(</body>)", $"{scriptElement}$1");
        contents.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StreamWriter(contents, null, -1, true);
        textWriter.Write(regex);
    }
}