using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Octokit.Bot
{
    public class EventContext
    {

        public EventContext(WebHookEvent webHookEvent, AccessToken accessToken, GitHubClient installationClient, GitHubClient appClient)
        {
            WebHookEvent = webHookEvent;
            InstallationAccessToken = accessToken;
            InstallationClient = installationClient;
            AppClient = appClient;
        }

        public WebHookEvent WebHookEvent { get; internal set; }

        public AccessToken InstallationAccessToken { get; internal set; }

        public GitHubClient InstallationClient { get; internal set; }

        public GitHubClient AppClient { get; internal set; }
    }
}
