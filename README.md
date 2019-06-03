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

At first, you need to define your GitHub application in GitHub's setting page. Detailed instructions are provided by GitHub [here](https://developer.github.com/apps/building-github-apps/creating-a-github-app/).
