using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using proceso_ETL.DATA.Extractors;
using proceso_ETL.DATA.Interfaces;
using proceso_ETL.DATA.Loaders;
using proceso_ETL.LOAD;
using proceso_ETL.LOAD.Services;
using proceso_ETL.PRESENTATION;

var builder = Host.CreateApplicationBuilder(args);

string relationalConn = builder.Configuration.GetConnectionString("RelationalDB") ?? "";
string dwConn = builder.Configuration.GetConnectionString("DataWarehouse") ?? "";
string apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://API.procesoETL.com";


builder.Services.AddSingleton<IDatabaseExtractor>(new DatabaseExtractor(relationalConn));
builder.Services.AddTransient<EtlOrchestratorService>();
builder.Services.AddHttpClient<IApiExtractor, APIExtractor>(client => {
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddTransient<ICsvExtractor, CsvExtractor>();
builder.Services.AddSingleton<IDataLoader>(new DataLoader(dwConn));
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();