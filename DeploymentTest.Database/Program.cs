using DeploymentTest.Database;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var appSettings = new AppSettings();

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, false);

builder.Build().Bind(appSettings);

var serviceProvider = CreateServices(appSettings);

using var scope = serviceProvider.CreateScope();

if (args.Length == 0)
    UpdateDatabase(scope.ServiceProvider);
else
{
    if (args[0] == "-down")
    {
        var version = Convert.ToInt64(args[1]);
        MigrateDown(scope.ServiceProvider, version);
    }
    else
    {
        Console.WriteLine("Incorrect parameters");
    }
}

static IServiceProvider CreateServices(AppSettings configuration)
{
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddMySql5()
            .WithGlobalCommandTimeout(TimeSpan.FromSeconds(10))
            .WithGlobalConnectionString(configuration.Db.ConnectionString)
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