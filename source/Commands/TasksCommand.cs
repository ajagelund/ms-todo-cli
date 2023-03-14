using Azure.Identity;
using HackTogether.Todo.Cli.Configuration;
using Microsoft.Graph;

namespace HackTogether.Todo.Cli.Commands;

public class TasksCommand
{
    private readonly GraphConfiguration graphConfiguration;

    public TasksCommand(GraphConfiguration graphConfiguration)
    {
        this.graphConfiguration = graphConfiguration;
    }

    public async Task<int> ExecuteAsync(string query, CancellationToken cancellationToken)
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
            var result = await graphClient.Me.Todo.Lists[query].Tasks.GetAsync(cancellationToken: cancellationToken);
            
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