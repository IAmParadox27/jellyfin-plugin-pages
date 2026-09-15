namespace Jellyfin.Plugin.PluginPages.JellyfinVersionSpecific
{
    public static class ScriptInjectionHelper
    {
        public static string GetPageInjection(this string[] scripts, string label)
        {
            return @$"""{label}"":[{scripts.First()}, [{string.Join(',', scripts.Skip(1))}]],";
        }
    }
}