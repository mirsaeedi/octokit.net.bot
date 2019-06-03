using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Octokit.Bot.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // 1. configure required parameters for WebHook handling to work
            services.Configure<GitHubOption>(Configuration.GetSection("github"));

            // 2. register webhook handlers
            services.AddScoped<IssueCommentEventHandler>();
            services.AddScoped<IssueEventHandler>();

            // 2. wire the handlers and corresponding events
            services.AddGitHubWebHookHandler(registry => registry
                .RegisterHandler<IssueCommentEventHandler>("issue_comment")
                .RegisterHandler<IssueEventHandler>("issue"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
