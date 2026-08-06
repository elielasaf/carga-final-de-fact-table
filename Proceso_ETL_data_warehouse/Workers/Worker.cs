using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using proceso_ETL.DATA.Extractors;
using proceso_ETL.DATA.Interfaces;
using proceso_ETL.DATA.Loaders;
using proceso_ETL.LOAD;
using proceso_ETL.LOAD.Services;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace proceso_ETL.PRESENTATION
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IApiExtractor _apiExtractor;
        private readonly ICsvExtractor _csvExtractor;
        private readonly EtlOrchestratorService _etlOrchestrator;
        private readonly IDatabaseExtractor _databaseExtractor; 

        public Worker(
            ILogger<Worker> logger,
            IDatabaseExtractor databaseExtractor,
            IApiExtractor apiExtractor,
            ICsvExtractor csvExtractor,
            EtlOrchestratorService etlOrchestrator)
        {
            _logger = logger;
            _databaseExtractor = databaseExtractor;
            _apiExtractor = apiExtractor;
            _csvExtractor = csvExtractor;
            _etlOrchestrator = etlOrchestrator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Iniciando proceso ETL a las: {time}", DateTimeOffset.Now);

            try
            {
                string stagingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StagingFiles");
                Directory.CreateDirectory(stagingPath);

                var customersDb = _databaseExtractor.ExtractDataFromView("View_Customers");
                if (customersDb.Rows.Count > 0)
                {
                    string path = Path.Combine(stagingPath, "Staging_DB_Customers.json");
                    var rowsList = customersDb.AsEnumerable().Select(row =>
                        customersDb.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? null : row[col])
                    ).ToList();
                    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(rowsList, new JsonSerializerOptions { WriteIndented = true }), stoppingToken);
                }

                await _etlOrchestrator.RunLoadProcessAsync();

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
