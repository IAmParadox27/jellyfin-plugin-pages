using System.Reflection;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.PluginPages.Model;
using MediaBrowser.Common.Net;

namespace Jellyfin.Plugin.PluginPages.Helpers
{
    public static class TransformationPatches
    {
        private static string[] s_userPluginPagesIds = new[]
        {
            "'./controllers/user/plugin/index.js'",
            "'user-plugin'"
        };

        private static string[] s_userPluginPagesHtmlIds = new[]
        {
            "'./controllers/user/plugin/index.html'",
            "'user-plugin-index-html'"
        };
        
        public static string IndexHtml(PatchRequestPayload payload)
        {
            NetworkConfiguration networkConfiguration = PluginPagesPlugin.Instance.ServerConfigurationManager.GetNetworkConfiguration();

            string rootPath = "";
            if (!string.IsNullOrWhiteSpace(networkConfiguration.BaseUrl))
            {
                rootPath = $"/{networkConfiguration.BaseUrl.TrimStart('/').Trim()}";
            }

            string scriptElement = $"<script plugin=\"PluginPages\" version=\"1.0.0.0\" src=\"{rootPath}/PluginPages/inject.js\" defer></script>";

            string regex = Regex.Replace(payload.Contents!, "(</body>)", $"{scriptElement}$1");

            return regex;
        }

        public static string SettingsHtml(PatchRequestPayload payload)
        {
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginPagesPlugin).Namespace}.Controller.userpluginsettings.html")!;
            using StreamReader textReader = new StreamReader(fileStream);
            
            return textReader.ReadToEnd();
        }

        public static string UserPluginJs(PatchRequestPayload payload)
        {
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginPagesPlugin).Namespace}.Controller.userpluginsettings.html")!;

            using StreamReader textReader = new StreamReader(fileStream);
            using StringWriter textWriter = new StringWriter();
            textWriter.WriteLine("\"use strict\";");
            textWriter.Write($"(self.webpackChunk = self.webpackChunk || []).push([[{s_userPluginPagesIds[1]}], {{{s_userPluginPagesIds[0]}:function(a,e,t){{t.r(e),e.default = '");
            textWriter.Write(textReader.ReadToEnd().Replace("\r", "").Replace("\n", "").Replace("'", "\\'"));
            textWriter.Write("'}}]);");
            
            return textWriter.ToString();
        }

        public static string UserPluginIndexHtml(PatchRequestPayload payload)
        {
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginPagesPlugin).Namespace}.Controller.userpluginsettings.html")!;

            using StreamReader textReader = new StreamReader(fileStream);
            using StringWriter textWriter = new StringWriter();
            textWriter.WriteLine("\"use strict\";");
            textWriter.Write($"(self.webpackChunk = self.webpackChunk || []).push([[{s_userPluginPagesHtmlIds[1]}], {{{s_userPluginPagesHtmlIds[0]}:function(a,e,t){{t.r(e),e.default = '");
            textWriter.Write(textReader.ReadToEnd().Replace("\r", "").Replace("\n", "").Replace("'", "\\'"));
            textWriter.Write("'}}]);");
            
            return textWriter.ToString();
        }

        public static string MainBundlePluginSettingsRoute(PatchRequestPayload payload)
        {
            string scriptElement = @"path:""userpluginsettings.html"",pageProps:{controller:""user/plugin/index"",view:""user/plugin/index.html""}},{";

            string regex = Regex.Replace(payload.Contents!, "(path:\"queue\")", $"{scriptElement}$1");
            
            return regex;
        }

        public static string MainBundleRouteIds(PatchRequestPayload payload)
        {
            string scriptElement = @$"""./user/plugin/index"":[{string.Join(',', s_userPluginPagesIds)}],";
            scriptElement += @$"""./user/plugin/index.html"":[{string.Join(',', s_userPluginPagesHtmlIds)}],";
            
            string regex = Regex.Replace(payload.Contents!, "(\"\\.\\/home\\.html\")", $"{scriptElement}$1");
            
            return regex;
        }

        public static string RuntimeBundle(PatchRequestPayload payload)
        {
            string scriptElement = @$"{s_userPluginPagesIds[1]}:""user-plugin"",";
            scriptElement += @$"{s_userPluginPagesHtmlIds[1]}:""user-plugin-index-html"",";
            
            string regex = Regex.Replace(payload.Contents!, "(8372:\"home-html\")", $"{scriptElement}$1");
            
            return regex;
        }
    }
}