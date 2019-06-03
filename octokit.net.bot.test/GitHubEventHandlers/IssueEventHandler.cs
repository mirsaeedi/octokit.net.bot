using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octokit.Bot.Test
{
    public class IssueEventHandler : IHookHandler
    {
        public async Task Handle(EventContext eventContext)
        {
            if (!eventContext.WebHookEvent.IsMessageAuthenticated)
            {
                // message is not issued by GitHub. Possibly from a malucious attacker.
                // log it and return;
                return;
            }
        }
    }
}
