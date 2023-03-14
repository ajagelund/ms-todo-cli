using System.Reflection;
using HackTogether.Todo.Cli.Application;
using HackTogether.Todo.Cli.Commands;
using HackTogether.Todo.Cli.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Build a config object, using env vars and JSON providers.
IConfiguration builder = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .Build();

var services = new ServiceCollection()
    .AddSingleton(builder.GetRequiredSection("Graph").Get<GraphConfiguration>() ?? new GraphConfiguration())
    .AddTransient<TaskListsCommand>()
    .AddTransient<TasksCommand>()
    .AddTransient<WhoCommand>()
    .AddTransient<TodoCli>();

var serviceProvider = services.BuildServiceProvider();
TodoCli application = serviceProvider.GetRequiredService<TodoCli>();

return application.RunApplication(args);
