using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Config;

namespace vite_api.Classes
{
    public class Copier
    {   
        private readonly IOptions<AppConfig> _appConfig;
        private readonly IServiceProvider _provider;
        
        private string Url => _appConfig.Value.NatsServerUrl ?? Defaults.Url;
        
        public Copier(IServiceProvider provider, IOptions<AppConfig> appConfig)
        {
            _provider = provider;
            _appConfig = appConfig;
        }
        public void CopyMessage(string streamName, ulong sequenceNumber)
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var jsm = connection.CreateJetStreamManagementContext();
            var msg = jsm.GetMessage(streamName, sequenceNumber);
        }
    }
}