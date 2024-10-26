using Api;
using Api.Events.EventDistributors;
using Api.Interfaces;
using Core.Logging;
using Core.Shared;
using Core.Shared.Interfaces;
using Database.Context;
using Database.Entities;
using Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHostBuilder host = Host.CreateDefaultBuilder(args);

host.ConfigureAppConfiguration((_, builder) =>
{
    builder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: true);
});

host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Trace);
});

host.ConfigureServices((context, services) =>
{
    services.Configure<Networking>(context.Configuration.GetSection("Networking"));
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(context.Configuration.GetSection("ConnectionString").Value));
    services.AddScoped<CharacterRepository>();
    services.AddSingleton<IEventDistributor, ModuleEventDistributor>();
    services.AddSingleton<IModule, Module>();
});

IHost build = host.Build();

try
{
    LogUtils.ShowWelcomeMessage();
    
    IEventDistributor eventDistributor = build.Services.GetRequiredService<IEventDistributor>();
    IModule module = build.Services.GetRequiredService<IModule>();
    
    eventDistributor.InitHandlerDistributor();
    module.Start();
    
    Character? character = await build.Services.GetRequiredService<CharacterRepository>().GetCharacterById(1);
    Logger.Information($"{character?.Name} has been initialized.");
    
    await build.RunAsync();
}
catch (Exception ex)
{
    Logger.Error($"ERROR: {ex.Message} [Application stopped] Details: {ex}");
}