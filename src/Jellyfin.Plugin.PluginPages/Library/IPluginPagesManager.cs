namespace Jellyfin.Plugin.PluginPages.Library
{
    public interface IPluginPagesManager
    {
        void RegisterPluginPage(PluginPage page);

        IEnumerable<PluginPage> GetPages();
    }

    public class PluginPage
    {
        public string? Id { get; set; }

        public string? Url { get; set; }

        public string? DisplayText { get; set; }

        public string? Icon { get; set; }
    }
}
