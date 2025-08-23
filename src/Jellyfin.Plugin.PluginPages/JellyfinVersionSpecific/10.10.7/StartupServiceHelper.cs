using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.PluginPages.JellyfinVersionSpecific
{
    public static class StartupServiceHelper
    {
        public static IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo()
            {
                Type = TaskTriggerInfo.TriggerStartup
            };
        }
    }
}