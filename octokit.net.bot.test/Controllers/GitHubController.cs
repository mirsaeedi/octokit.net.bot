using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octokit.Bot;

namespace Octokit.Bot.Test
{
    [Route("github")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly WebHookHandlerRegistry _registry;

        public GitHubController(WebHookHandlerRegistry registry)
        {
            _registry = registry;
        }

        [HttpPost("hooks")]
        public async Task<ActionResult> HandleGitHubHooks(WebHookEvent webhookEvent)
        {
            await _registry.Handle(webhookEvent);

            return Ok();
        }

    }
}
