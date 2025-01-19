using System.Reflection;
using Jellyfin.Plugin.PluginPages.Library;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.PluginPages.Controller
{
    /// <summary>
    /// Plugin Pages API controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PluginPagesController : ControllerBase
    {
        private readonly IPluginPagesManager m_pluginPagesManager;
        private readonly Assembly m_assembly;
        private readonly string m_scriptPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pluginPagesManager">Instance of <see href="IPluginPagesManager" /> interface.</param>
        public PluginPagesController(IPluginPagesManager pluginPagesManager)
        {
            m_assembly = Assembly.GetExecutingAssembly();
            m_pluginPagesManager = pluginPagesManager;
            m_scriptPath = GetType().Namespace + ".inject.js";
        }

        /// <summary>
        /// Get pages this plugin serves for users.
        /// </summary>
        /// <returns>Array of <see cref="PluginPage"/>.</returns>
        [HttpGet("User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult<PluginPage>> GetPluginPages()
        {
            List<PluginPage> pages = m_pluginPagesManager.GetPages().ToList();

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
    }
}
