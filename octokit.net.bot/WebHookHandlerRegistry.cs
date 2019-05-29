using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Octokit.Bot
{
    public class WebHookHandlerRegistry
    {
        public delegate Task HandleWebHook(EventContext eventContext);

        private readonly GitHubOption _gitHubOption;
        private readonly Dictionary<string, List<HandleWebHook>> _registry = new Dictionary<string, List<HandleWebHook>>();

        public WebHookHandlerRegistry(GitHubOption gitHubOption)
        {
            _gitHubOption = gitHubOption;
        }

        public void RegisterHandler(string eventName, HandleWebHook webHookHandler)
        {
            if (!_registry.ContainsKey(eventName))
            {
                _registry[eventName] = new List<HandleWebHook>();
            }

            _registry[eventName].Add(webHookHandler);
        }

        public void UnRegisterHandler(string eventName, HandleWebHook webHookHandler)
        {
            if (_registry.ContainsKey(eventName))
            {
                _registry[eventName].Remove(webHookHandler);
            }
        }

        public async Task Handle(WebHookEvent webhookEvent)
        {
            var installationId = webhookEvent.GetInstallationId();

            var appClient = GitHubClientFactory.CreateGitHubAppClient(_gitHubOption);
            var installationContext = !installationId.HasValue
            ? (null,null)
            :await GitHubClientFactory.CreateGitHubInstallationClient(appClient,installationId.Value, _gitHubOption.AppName);

            var handlers = _registry.ContainsKey(webhookEvent.GitHubEvent)? _registry[webhookEvent.GitHubEvent] :null;

            if (handlers != null)
            {
                var context = new EventContext(webhookEvent, installationContext.AccessToken, installationContext.Client, appClient);
                foreach (var handler in handlers)
                {
                    await handler(context);
                }
            }
                
        }
    }
}
