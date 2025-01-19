using Jellyfin.Plugin.PluginPages.Library;

namespace Jellyfin.Plugin.PluginPages.Manager
{
    /// <inheritdoc/>
    public class PluginPagesManager : IPluginPagesManager
    {
        private List<PluginPage> m_pluginPages = new List<PluginPage>();

        /// <inheritdoc/>
        public IEnumerable<PluginPage> GetPages()
        {
            return m_pluginPages;
        }

        /// <inheritdoc/>
        public void RegisterPluginPage(PluginPage page)
        {
            if (m_pluginPages.Any(x => x.Id == page.Id))
            {
                // The page is already added
                // TODO: Log error
                return;
            }

            m_pluginPages.Add(page);
        }
    }
}
