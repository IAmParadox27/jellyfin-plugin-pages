using Jellyfin.Plugin.PluginPages.Library;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages
{
    public static class PluginInterface
    {
        public static void RegisterPage(JObject payload)
        {
            PluginPage? pageEntry = payload.ToObject<PluginPage>();

            if (pageEntry != null)
            {
                PluginPagesPlugin.Instance.PluginPagesManager.RegisterPluginPage(pageEntry);
            }
        }
        
        public static void RemovePage(string id)
        {
            PluginPagesPlugin.Instance.PluginPagesManager.RemovePage(id);
        }
    }
}