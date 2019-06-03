# Octokit.Bot

Creating GitHub Apps using C# and ASP.NET Core is easy using Octokit.Bot. This library takes care of lots of boiler plate code and lets you focus on nothing except your own problem.

# Development Environment

If you want to test your bot inside development environment, you need a way to route GitHub's webhooks to your development machine. Since your development machine does not have a static IP address that is exposed to internet, you have to use the awesome tool provided by [Probot team](https://github.com/probot/probot) called [smee.io](https://github.com/probot/smee-client).

Follow the step 1 as described [here](https://developer.github.com/apps/quickstart-guides/setting-up-your-development-environment/#step-1-start-a-new-smee-channel) to open an smee channel.

## smee.io and ASP.NET Core

It seems you need to do some tweaks to make smee.io works with your ASP.NET Core web application.

 1. Disable HTTPS in your develop environment. To do So, you can go to the Propertise page of your web application, Then click on debug and diselect _Enable SSL_.
 2. Make your web app to load at port 3000 and IP 127.0.0.1 (http://127.0.0.1:3000). To do So, you can go to the Propertise page of your web application, Then click on debug and change the _App URL_ to _http://127.0.0.1:3000_.

# GitHub Apps

At first, you need to define your GitHub application in GitHub's setting page. Follow the instructions of [Step 2](https://developer.github.com/apps/quickstart-guides/setting-up-your-development-environment/#step-2-register-a-new-github-app) from GitHub tutorial. Detailed instructions are also provided by GitHub [here](https://developer.github.com/apps/building-github-apps/creating-a-github-app/).

# Configuration

Octokit requires _AppName_, _AppIdentifier_, _WebHookSecret_, and your app's _PrivateKey_. You need to provide these values through ASP.NET configuration mechanism. For example the _appsettings.json_ should be like the following:

```json
{
  "github": {
    "WebHookSecret": "",
    "AppIdentifier": "",
    "PrivateKey": "",
    "AppName": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}

```

I'd recommend putting the private key as an environment variable with the name **github__privatekey** instead of having it inside _appsettings.json_.

Next, in the _startup_ class inside the _ConfigureServices_ add the following line:

```C#
services.Configure<GitHubOption>(Configuration.GetSection("github"));
```

The **GitHubOption** nelongs to Octokit.Bot and holds the required information. 

# Event Handlers

To implement an event handler you should create a class that inherits from **IHookHandler**. The following Handler is going to Handle [_IssueCommentEvent_](https://developer.github.com/v3/activity/events/types/#issuecommentevent). Using the _GetPayload()_ method of _EventContext_ you can query the payload with the same structure defined by GitHub. Since the access is provided dynamically, intellisense is not available in your IDE.

```C#
public class IssueCommentEventHandler : IHookHandler
    {
        public async Task Handle(EventContext eventContext)
        {
        
            // check if the message is issued by github
            if (!eventContext.WebHookEvent.IsMessageAuthenticated)
            {
                // message is not issued by GitHub. Possibly from a malucious attacker.
                // log it and return;
                return;
            }

            // use GetPayload() to access the json payload as defined by GitHub
            
            var action = eventContext.WebHookEvent.GetPayload().action;

            if (action != "created")
                return;

            var body = (string)eventContext.WebHookEvent.GetPayload().comment.body;

            body = body.Trim();

            var authorAssociation = (string)eventContext.WebHookEvent.GetPayload().comment.author_association;

            // do your logic here
        }
    }
```

# Interating with GitHub

If you want to send a command to GitHub such as submitting a comment in one of the repositories, you can do so by using the [Octokit.net](https://github.com/octokit/octokit.net) client that is provided by Octokit.Bot. As you can see in the [IssueCommentEventHandler] we can submit a comment using the **Client** property of **InstallationContext**. This client has a full access to the repository or organization that has installed your application.

```C#

var issueNumber = (int)eventContext.WebHookEvent.GetPayload().issue.number;
var repositoryId = (long)eventContext.WebHookEvent.GetPayload().repository.id;

var commentResponse = await eventContext.InstallationContext
                                       .Client
                                       .Issue.Comment
                                       .Create(repositoryId, issueNumber, "Hello There");

```

# 
