using Azure.Identity;
using HackTogether.Todo.Cli.Configuration;
using Microsoft.Graph;
using static Microsoft.Graph.Me.Todo.Lists.ListsRequestBuilder;

namespace HackTogether.Todo.Cli.Commands;

public class TasksCommand
{
    private readonly GraphConfiguration graphConfiguration;

    public TasksCommand(GraphConfiguration graphConfiguration)
    {
        this.graphConfiguration = graphConfiguration;
    }

    public async Task<int> ExecuteAsync(string? listName, string? id, CancellationToken cancellationToken)
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

            if (id == null)
            {   Action<ListsRequestBuilderGetRequestConfiguration>? requestConfiguration = configuration =>
                {
                    configuration.QueryParameters.Filter = $"contains(displayName, '{listName}')";
                };
            
                var listResult = await graphClient.Me.Todo.Lists.GetAsync(requestConfiguration, cancellationToken);
                var firstList = listResult.Value.FirstOrDefault();

                id = firstList?.Id;
            }

            var result = await graphClient.Me.Todo.Lists[id].Tasks.GetAsync(cancellationToken: cancellationToken);
            
            var todoTasks = result?.Value;
            if (todoTasks?.Count == 0)
            {
                Console.WriteLine("No tasks found.");
                return 0;
            }

            Console.WriteLine($"\nYour tasks ({todoTasks!.Count}):\n");
            foreach (var todoTask in todoTasks!)
            {
                Console.WriteLine($" = {todoTask.Title}");
            }
            return 0;
        }
        catch
        {
            return 1;
        }
    }
}