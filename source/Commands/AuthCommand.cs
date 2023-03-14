using Azure.Identity;
using HackTogether.Todo.Cli.Configuration;
using Microsoft.Graph;

namespace HackTogether.Todo.Cli.Commands;

public class AuthCommand
{
    private readonly GraphConfiguration graphConfiguration;

    public AuthCommand(GraphConfiguration graphConfiguration)
    {
        this.graphConfiguration = graphConfiguration;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var scopes = new[] {"User.Read", "Tasks.Read"};
            var interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions
            {
                ClientId = graphConfiguration?.ClientId
            };
            var tokenCredential = new InteractiveBrowserCredential(interactiveBrowserCredentialOptions);

            var graphClient = new GraphServiceClient(tokenCredential, scopes);

            var me = await graphClient.Me.GetAsync();
            Console.WriteLine($"Hello {me?.DisplayName}!");
            return 0;
        }
        catch
        {
            return 1;
        }
    }
}