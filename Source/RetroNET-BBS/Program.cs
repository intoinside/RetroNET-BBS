using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parser.Markdown;
using Parser.Raw;
using RetroNET_BBS.Client;
using RetroNET_BBS.ContentProvider;
using RetroNET_BBS.Server;
using Common;

public class Prg
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile(Constants.Config.AppSettingsFile, optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(configure => configure.AddConsole());
                
                // Register PageContainer as Singleton (for now, until further refactoring)
                // Note: PageContainer is static, so we don't strictly need to register it, 
                // but we should initialize it here.
                
                // Register Servers as Hosted Services
                services.AddSingleton<IUserFactory, UserFactory>();
                services.AddHostedService<BbsBackgroundService>();
            })
            .Build();

        var config = host.Services.GetRequiredService<IConfiguration>();
        var logger = host.Services.GetRequiredService<ILogger<Prg>>();

        logger.LogInformation("Hello, World! This is RetroNET-BBS!");

        var folder = config[Constants.Config.PathKey];
        
        logger.LogInformation("Parsing pages...");
        PageContainer.Pages = Markdown.ParseAllFiles(folder);

        logger.LogInformation("Parsing imports...");
        PageContainer.Imports = Seq.ParseAllFiles(folder);

        await host.RunAsync();

        logger.LogInformation("Goodbye, World!");
    }
}

public class BbsBackgroundService : BackgroundService
{
    private readonly ILogger<BbsBackgroundService> logger;
    private readonly IUserFactory userFactory;

    public BbsBackgroundService(ILogger<BbsBackgroundService> logger, IUserFactory userFactory)
    {
        this.logger = logger;
        this.userFactory = userFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting servers...");

        var petsciiServer = new Server("0.0.0.0", 8502, ConnectionType.Petscii, userFactory);
        var telnetServer = new Server("0.0.0.0", 23, ConnectionType.Telnet, userFactory);

        var t1 = petsciiServer.Start(stoppingToken);
        var t2 = telnetServer.Start(stoppingToken);

        await Task.WhenAll(t1, t2);
    }
}