using System.Reflection;
using System.Runtime.Loader;
using Jellyfin.Plugin.PluginPages.Library;
using Jellyfin.Plugin.PluginPages.Manager;
using Jellyfin.Plugin.Referenceable.Services;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PluginPages
{
    public class PluginPagesServiceRegistrator : PluginServiceRegistrator
    {
        public override void ConfigureServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.StartsWith("Jellyfin.Plugin.FileTransformation"))
                {
                    Assembly? assembly = AssemblyLoadContext.All.Where(x => !x.IsCollectible).SelectMany(x => x.Assemblies)
                        .FirstOrDefault(x => x.FullName == args.Name);
                    // Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
                    //     .FirstOrDefault(x => x.FullName == args.Name);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                    
                    string pluginsDir = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                    string fileTransformationDir = Directory.GetDirectories(pluginsDir, "File Transformation_*", SearchOption.TopDirectoryOnly).OrderBy(
                        x =>
                        {
                            return Version.Parse(x.Split('_').LastOrDefault());
                        }).Last();
                    
                    string dll = Path.Combine(fileTransformationDir, "Jellyfin.Plugin.FileTransformation.dll");
                    
                    return Assembly.LoadFile(dll);
                }
                
                return null;
            };
            
            serviceCollection.AddSingleton<IPluginPagesManager, PluginPagesManager>();
        }
    }
}