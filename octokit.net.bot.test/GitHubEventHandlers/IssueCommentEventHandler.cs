using System.Threading.Tasks;

namespace Octokit.Bot.Test
{
    public class IssueCommentEventHandler : IHookHandler
    {
        public async Task Handle(EventContext eventContext)
        {
            if (!eventContext.WebHookEvent.IsMessageAuthenticated)
            {
                // message is not issued by GitHub. Possibly from a malucious attacker.
                // log it and return;
                return;
            }

            var action = eventContext.WebHookEvent.GetPayload().action;

            if (action != "created")
                return;

            var body = (string)eventContext.WebHookEvent.GetPayload().comment.body;

            body = body.Trim();

            var authorAssociation = (string)eventContext.WebHookEvent.GetPayload().comment.author_association;

            if (body.StartsWith("Hello @Octokit.Bot") && authorAssociation == "OWNER")
            {
                var issueNumber = (int)eventContext.WebHookEvent.GetPayload().issue.number;
                var repositoryId = (long)eventContext.WebHookEvent.GetPayload().repository.id;

                var commentResponse = await eventContext.InstallationContext
                                       .Client
                                       .Issue.Comment
                                       .Create(repositoryId, issueNumber, "Hello There");
            }

        }
    }
}
