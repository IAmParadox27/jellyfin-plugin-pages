using System.Reflection;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.PluginPages.Library;
using Jellyfin.Plugin.PluginPages.Model;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.PluginPages.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class PluginPagesController : ControllerBase
    {
        private readonly IPluginPagesManager m_pluginPagesManager;
        private readonly Assembly m_assembly;
        private readonly string m_scriptPath;

        private string[] m_userPluginPagesIds = new[]
        {
            "'./controllers/user/plugin/index.js'",
            "'user-plugin'"
        };

        private string[] m_userPluginPagesHtmlIds = new[]
        {
            "'./controllers/user/plugin/index.html'",
            "'user-plugin-index-html'"
        };

        public PluginPagesController(IPluginPagesManager pluginPagesManager)
        {
            m_assembly = Assembly.GetExecutingAssembly();
            m_pluginPagesManager = pluginPagesManager;
            m_scriptPath = GetType().Namespace + ".inject.js";
        }

        [HttpGet("User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult<PluginPage>> GetPluginPages()
        {
            List<PluginPage> pages = m_pluginPagesManager.GetPages()
                .Select(x =>
                {
                    string scheme = Request.Scheme;
                    IHeaderDictionary requestHeaders = Request.GetTypedHeaders().Headers;
                    if (requestHeaders.ContainsKey("X-Forwarded-Proto"))
                    {
                        scheme = requestHeaders["X-Forwarded-Proto"]!;
                    }

                    string? url = x.Url;
                    if (x.Url?.StartsWith("/") ?? true)
                    {
                        url = $"{scheme}://{Request.Host.Value}{x.Url}";
                    }

                    return new PluginPage
                    {
                        Id = x.Id,
                        Icon = x.Icon,
                        DisplayText = x.DisplayText,
                        Url = url
                    };
                }).ToList();

            return new QueryResult<PluginPage>(
                0,
                pages.Count,
                pages);
        }

        [HttpGet("inject.js")]
        [Produces("application/javascript")]
        public ActionResult GetPluginPagesInjectionScript()
        {
            Stream? scriptStream = m_assembly.GetManifestResourceStream(m_scriptPath);

            if (scriptStream != null)
            {
                return File(scriptStream, "application/javascript");
            }
            
            return NotFound();
        }

        [HttpPost("Patch/IndexHtml")]
        public ActionResult PatchIndexHtml([FromBody] PatchRequestPayload payload)
        {
            string scriptElement = "<script plugin=\"PluginPages\" version=\"1.0.0.0\" src=\"/PluginPages/inject.js\" defer></script>";

            string regex = Regex.Replace(payload.Contents!, "(</body>)", $"{scriptElement}$1");

            return Content(regex, "text/html");
        }
        
        [HttpPost("Patch/UserPlugin/SettingsHtml")]
        public ActionResult PatchUserPluginSettingsHtml([FromBody] PatchRequestPayload payload)
        {
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginPagesPlugin).Namespace}.Controller.userpluginsettings.html")!;
            using StreamReader textReader = new StreamReader(fileStream);

            return Content(textReader.ReadToEnd(), "text/html");
        }
        
        [HttpPost("Patch/UserPlugin/Javascript")]
        public ActionResult PatchUserPluginJavascript([FromBody] PatchRequestPayload payload)
        {
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginPagesPlugin).Namespace}.Controller.userpluginsettings.html")!;

            using StreamReader textReader = new StreamReader(fileStream);
            using StringWriter textWriter = new StringWriter();
            textWriter.WriteLine("\"use strict\";");
            textWriter.Write($"(self.webpackChunk = self.webpackChunk || []).push([[{m_userPluginPagesIds[1]}], {{{m_userPluginPagesIds[0]}:function(a,e,t){{t.r(e),e.default = '");
            textWriter.Write(textReader.ReadToEnd().Replace("\r", "").Replace("\n", "").Replace("'", "\\'"));
            textWriter.Write("'}}]);");
            
            return Content(textWriter.ToString(), "application/javascript");
        }
        
        [HttpPost("Patch/UserPlugin/IndexHtmlChunk")]
        public ActionResult PatchUserPluginIndexHtmlJavascript([FromBody] PatchRequestPayload payload)
        {
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginPagesPlugin).Namespace}.Controller.userpluginsettings.html")!;

            using StreamReader textReader = new StreamReader(fileStream);
            using StringWriter textWriter = new StringWriter();
            textWriter.WriteLine("\"use strict\";");
            textWriter.Write($"(self.webpackChunk = self.webpackChunk || []).push([[{m_userPluginPagesHtmlIds[1]}], {{{m_userPluginPagesHtmlIds[0]}:function(a,e,t){{t.r(e),e.default = '");
            textWriter.Write(textReader.ReadToEnd().Replace("\r", "").Replace("\n", "").Replace("'", "\\'"));
            textWriter.Write("'}}]);");
            
            return Content(textWriter.ToString(), "application/javascript");
        }
        
        [HttpPost("Patch/MainBundle/PluginSettingsRoute")]
        public ActionResult PatchMainBundle_PluginSettingsRoute([FromBody] PatchRequestPayload payload)
        {
            string scriptElement = @"path:""userpluginsettings.html"",pageProps:{controller:""user/plugin/index"",view:""user/plugin/index.html""}},{";

            string regex = Regex.Replace(payload.Contents!, "(path:\"queue\")", $"{scriptElement}$1");
            
            return Content(regex, "application/javascript");
        }
        
        [HttpPost("Patch/MainBundle/RouteIds")]
        public ActionResult PatchMainBundle_RouteIds([FromBody] PatchRequestPayload payload)
        {
            string scriptElement = @$"""./user/plugin/index"":[{string.Join(',', m_userPluginPagesIds)}],";
            scriptElement += @$"""./user/plugin/index.html"":[{string.Join(',', m_userPluginPagesHtmlIds)}],";
            
            string regex = Regex.Replace(payload.Contents!, "(\"\\.\\/home\\.html\")", $"{scriptElement}$1");
            
            return Content(regex, "application/javascript");
        }
        
        [HttpPost("Patch/RuntimeBundle")]
        public ActionResult PatchRuntimeBundle([FromBody] PatchRequestPayload payload)
        {
            string scriptElement = @$"{m_userPluginPagesIds[1]}:""user-plugin"",";
            scriptElement += @$"{m_userPluginPagesHtmlIds[1]}:""user-plugin-index-html"",";
            
            string regex = Regex.Replace(payload.Contents!, "(8372:\"home-html\")", $"{scriptElement}$1");
            
            return Content(regex, "application/javascript");
        }
    }
}
