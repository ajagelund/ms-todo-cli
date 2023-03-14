using HackTogether.Todo.Cli.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace HackTogether.Todo.Cli.Application;

public class TodoCli
{
    private readonly TaskListsCommand taskListsCommand;
    private readonly TasksCommand tasksCommand;
    private readonly WhoCommand whoCommand;

    public TodoCli(
        TaskListsCommand taskListsCommand,
        TasksCommand tasksCommand,
        WhoCommand whoCommand)
    {
        this.taskListsCommand = taskListsCommand;
        this.tasksCommand = tasksCommand;
        this.whoCommand = whoCommand;
    }

    public int RunApplication(params string[] args)
    {
        var app = new CommandLineApplication()
        {
            Name = "todo"
        };

        app.HelpOption(inherited: true);
        
        app.Command("list", listCmd =>
        {
            listCmd.Description = "Show all task lists";

            var filter = listCmd.Option<string>("-f|--filter <NAME>", "Display name filter", CommandOptionType.SingleValue);
            filter.DefaultValue = "";

            listCmd.OnExecuteAsync(token => taskListsCommand.ExecuteAsync(filter.Value(), token));
        });

        app.Command("task", taskCmd =>
        {
            taskCmd.Description = "Show all tasks in a given list";

            var listId = taskCmd.Argument("id", "List id");

            taskCmd.OnExecuteAsync(token => tasksCommand.ExecuteAsync(listId.Value ?? string.Empty, token));
        });

        app.Command("who", listCmd =>
        {
            listCmd.Description = "Show logged in user";

            listCmd.OnExecuteAsync(token => whoCommand.ExecuteAsync(token));
        });

        app.OnExecute(() =>
        {
            Console.WriteLine("Specify a subcommand");
            app.ShowHelp();
            return 0;
        });

        return app.Execute(args);
    }
}