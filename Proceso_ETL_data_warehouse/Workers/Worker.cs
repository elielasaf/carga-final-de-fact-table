using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using proceso_ETL.DATA.Interfaces;
using proceso_ETL.LOAD.Services;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace proceso_ETL.PRESENTATION
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Iniciando proceso ETL a las: {time}", DateTimeOffset.Now);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var databaseExtractor = scope.ServiceProvider.GetRequiredService<IDatabaseExtractor>();
                    var etlOrchestrator = scope.ServiceProvider.GetRequiredService<EtlOrchestratorService>();

                    string stagingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StagingFiles");
                    Directory.CreateDirectory(stagingPath);

                    var customersDb = databaseExtractor.ExtractDataFromView("View_Customers");
                    if (customersDb.Rows.Count > 0)
                    {
                        string path = Path.Combine(stagingPath, "Staging_DB_Customers.json");
                        var rowsList = customersDb.AsEnumerable().Select(row =>
                            customersDb.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? null : row[col])
                        ).ToList();
                        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(rowsList, new JsonSerializerOptions { WriteIndented = true }), stoppingToken);
                    }

                    await etlOrchestrator.RunLoadProcessAsync();
                }

                stopwatch.Stop();
                _logger.LogInformation("Proceso ETL completo finalizado exitosamente en {elapsedTime} ms.", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error crítico durante el proceso ETL.");
            }
        }
    }
}