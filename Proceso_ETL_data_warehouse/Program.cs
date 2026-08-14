using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using proceso_ETL.DATA.Extractors;
using proceso_ETL.DATA.Interfaces;
using proceso_ETL.DATA.Loaders;
using proceso_ETL.LOAD.Services;
using proceso_ETL.PRESENTATION;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        string relationalConn = configuration.GetConnectionString("RelationalDB") ?? "";
        string dwConn = configuration.GetConnectionString("DataWarehouse") ?? "";
        string apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://API.procesoETL.com";

        services.AddTransient<IDatabaseExtractor>(sp => new DatabaseExtractor(relationalConn));
        services.AddTransient<IDataLoader>(sp => new DataLoader(dwConn));
        services.AddTransient<EtlOrchestratorService>();

        services.AddHttpClient<IApiExtractor, APIExtractor>(client => {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        services.AddTransient<ICsvExtractor, CsvExtractor>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();