using System.Collections.Concurrent;
using System.IO.Pipes;
using Microsoft.Extensions.Hosting;

namespace Jellyfin.Plugin.PluginPages.Services
{
    public class NamedPipeService : IHostedService
    {
        private ConcurrentBag<NamedPipeServerStream> m_activeStreams = new ConcurrentBag<NamedPipeServerStream>();
        private bool m_running = false;
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            m_running = true;
            
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            m_running = false;
            
            while (m_activeStreams.Any(x => x.IsConnected))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
            }

            foreach (NamedPipeServerStream stream in m_activeStreams)
            {
                await stream.DisposeAsync();
            }
        
            m_activeStreams.Clear();
        }

        public async void CreateNamedPipeHandler(string pipeName, Func<NamedPipeServerStream, Task> callback)
        {
            while (m_running)
            {
                NamedPipeServerStream pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 254);
                await pipeStream.WaitForConnectionAsync();
                
                m_activeStreams.Add(pipeStream);
                
                AsyncCallback(pipeStream, callback);
            }
        }

        private async void AsyncCallback(NamedPipeServerStream pipeStream, Func<NamedPipeServerStream, Task> callback)
        {
            await callback(pipeStream);
        }
    }
}