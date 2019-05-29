using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Octokit.Bot
{
    public class WebhookEventBinder : IModelBinder
    {
        private IOptions<GitHubOption> _gitHubOptions;

        public WebhookEventBinder(IOptions<GitHubOption> gitHubOptions)
        {
            _gitHubOptions = gitHubOptions;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var webHookEvent = new WebHookEvent(_gitHubOptions.Value);

            PopulateGitHubHeaders(bindingContext, webHookEvent);

            using (var sr = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                webHookEvent.PayloadRaw = sr.ReadToEnd();
            }

            bindingContext.Result = ModelBindingResult.Success(webHookEvent);

            return Task.CompletedTask;
        }

        private void PopulateGitHubHeaders(ModelBindingContext bindingContext, WebHookEvent webHookEvent)
        {
            webHookEvent.GitHubEvent = bindingContext.HttpContext.Request.Headers["x-github-event"];
            webHookEvent.GitHubDelivery = bindingContext.HttpContext.Request.Headers["x-gitHub-delivery"];
            webHookEvent.HubSignature = bindingContext.HttpContext.Request.Headers["X-hub-signature"];
        }
    }
}
