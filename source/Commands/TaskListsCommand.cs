using Azure.Identity;
using HackTogether.Todo.Cli.Configuration;
using Microsoft.Graph;

namespace HackTogether.Todo.Cli.Commands;

public class TaskListsCommand
{
    private readonly GraphConfiguration graphConfiguration;

    public TaskListsCommand(GraphConfiguration graphConfiguration)
    {
        this.graphConfiguration = graphConfiguration;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
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
            var result = await graphClient.Me.Todo.Lists.GetAsync(cancellationToken: cancellationToken);
            
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