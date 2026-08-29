using System.Reflection;
using System.Runtime.Loader;
using Jellyfin.Plugin.PluginPages.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.PluginPages.Manager
{
    public class PluginPagesManager : IPluginPagesManager
    {
        private List<PluginPage> m_pluginPages = new List<PluginPage>();
        private readonly IServiceProvider m_serviceProvider;

        public PluginPagesManager(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider;
        }
        
        public IEnumerable<PluginPage> GetPages()
        {
            return m_pluginPages.Where(x =>
            {
                if (x.IsEnabledAssembly != null && x.IsEnabledClass != null && x.IsEnabledMethod != null)
                {
                    Type? isEnabledClass = AssemblyLoadContext.All.SelectMany(y => y.Assemblies)
                        .FirstOrDefault(y => y.FullName?.Contains(x.IsEnabledAssembly) ?? false)?.GetTypes()
                        .FirstOrDefault(y => y.Name == x.IsEnabledClass);

                    MethodInfo? isEnabledMethod = isEnabledClass?.GetMethod(x.IsEnabledMethod);

                    if (isEnabledMethod?.Invoke(null, new object?[] { x.Id }) is bool isEnabled)
                    {
                        return isEnabled;
                    }
                }

                return true;
            });
        }

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
        
        public void RemovePage(string id)
        {
            m_pluginPages.RemoveAll(x => x.Id == id);
        }
    }
}
