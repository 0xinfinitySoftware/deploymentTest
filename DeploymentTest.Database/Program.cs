using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory());


Console.WriteLine(args.Length);
if (args.Length < 5)
{
    throw new ArgumentException("Invalid parameters");
}

var connectionString = $"Server={args[0]};Uid={args[1]};Database={args[2]};Pwd={args[3]};Port={args[4]};";
Console.WriteLine(connectionString);

builder.Build().Bind(connectionString);

var serviceProvider = CreateServices(connectionString);

using var scope = serviceProvider.CreateScope();
 
if (args.Length == 5)
{
    Console.WriteLine("Updating database");
    UpdateDatabase(scope.ServiceProvider);
}
else
{
    if (args[5] == "-down")
    {
        var version = Convert.ToInt64(args[6]);
        Console.WriteLine($"Migrating down to version {version}");

        MigrateDown(scope.ServiceProvider, version);
    }
    else
    {
        Console.WriteLine("Incorrect parameters");
    }
}

static IServiceProvider CreateServices(string connectionString)
{
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddMySql5()
            .WithGlobalCommandTimeout(TimeSpan.FromSeconds(10))
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(Program).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}

static void UpdateDatabase(IServiceProvider serviceProvider)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

static void MigrateDown(IServiceProvider serviceProvider, long version)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateDown(version);
}