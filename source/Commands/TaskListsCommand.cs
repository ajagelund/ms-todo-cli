using Azure.Identity;
using HackTogether.Todo.Cli.Configuration;
using Microsoft.Graph;
using static Microsoft.Graph.Me.Todo.Lists.ListsRequestBuilder;

namespace HackTogether.Todo.Cli.Commands;

public class TaskListsCommand
{
    private readonly GraphConfiguration graphConfiguration;

    public TaskListsCommand(GraphConfiguration graphConfiguration)
    {
        this.graphConfiguration = graphConfiguration;
    }

    public async Task<int> ExecuteAsync(string? filter, CancellationToken cancellationToken)
    {
        try
        {
            var scopes = new[] { "User.Read", "Tasks.Read" };
            var interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions
            {
                ClientId = graphConfiguration?.ClientId
            };
            var tokenCredential = new InteractiveBrowserCredential(interactiveBrowserCredentialOptions);

            var graphClient = new GraphServiceClient(tokenCredential, scopes);
            Action<ListsRequestBuilderGetRequestConfiguration>? requestConfiguration = configuration =>
            {
                configuration.QueryParameters.Filter = $"contains(displayName, '{filter}')";
            };
            
            var result = await graphClient.Me.Todo.Lists.GetAsync(requestConfiguration, cancellationToken);
            
            var todoTaskLists = result?.Value;
            if (todoTaskLists?.Count == 0)
            {
                Console.WriteLine("No lists found.");
                return 0;
            }

            Console.WriteLine($"\nYour lists ({todoTaskLists!.Count}):\n");
            foreach (var todoTaskList in todoTaskLists!)
            {
                Console.WriteLine($" = {todoTaskList.DisplayName}");
            }
            return 0;
        }
        catch
        {
            return 1;
        }
    }
}