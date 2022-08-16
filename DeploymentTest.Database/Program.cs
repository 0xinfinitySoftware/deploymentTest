using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory());


if (args.Length < 1)
{
    throw new ArgumentException("Invalid parameters");
}

var connectionString = args[0];

Console.WriteLine(connectionString);

builder.Build().Bind(connectionString);

var serviceProvider = CreateServices(connectionString);

using var scope = serviceProvider.CreateScope();

if (args.Length == 1)
{
    Console.WriteLine("Updating database");
    UpdateDatabase(scope.ServiceProvider);
}
else
{
    if (args[1] == "-down" && args.Length == 3)
    {
        var version = Convert.ToInt64(args[2]);
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