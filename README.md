# Octokit.Bot

Creating GitHub Apps using C# and ASP.NET Core is easy using Octokit.Bot. This library takes care of lots of boiler plate code and lets you focus on nothing except your own problem.

# Development Environment

If you want to test your bot inside development environment, you need a way to route GitHub's webhooks to your development machine. Since your development machine does not have a static IP address that is exposed to internet, you have to use the awesome tool provided by [Probot team](https://github.com/probot/probot) called [smee.io](https://github.com/probot/smee-client)

# GitHub Apps

At first, you need to define your GitHub application in GitHub's setting page. Detailed instructions are provided by GitHub [here](https://developer.github.com/apps/building-github-apps/creating-a-github-app/).
