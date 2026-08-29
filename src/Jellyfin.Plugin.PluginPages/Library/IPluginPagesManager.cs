namespace Jellyfin.Plugin.PluginPages.Library
{
    public interface IPluginPagesManager
    {
        void RegisterPluginPage(PluginPage page);
        
        void RemovePage(string id);

        IEnumerable<PluginPage> GetPages();
    }

    public class PluginPage
    {
        public string? Id { get; set; }

        public string? Url { get; set; }

        public string? DisplayText { get; set; }

        public string? Icon { get; set; }
        
        public string? IsEnabledAssembly { get; set; }
        
        public string? IsEnabledClass { get; set; }
        
        public string? IsEnabledMethod { get; set; }
    }
}
