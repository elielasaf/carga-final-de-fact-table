using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using proceso_ETL.DATA.Extractors;
using proceso_ETL.DATA.Loaders;
using proceso_ETL.LOAD;
using System.Diagnostics;
using System.Text.Json;
using System.Data;

namespace proceso_ETL.PRESENTATION
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DatabaseExtractor _databaseExtractor;
        private readonly APIExtractor _apiExtractor;
        private readonly CsvExtractor _csvExtractor;

        public Worker(
            ILogger<Worker> logger,
            DatabaseExtractor databaseExtractor,
            APIExtractor apiExtractor,
            CsvExtractor csvExtractor)
        {
            _logger = logger;
            _databaseExtractor = databaseExtractor;
            _apiExtractor = apiExtractor;
            _csvExtractor = csvExtractor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Iniciando proceso de Extracción (E de ETL) a las: {time}", DateTimeOffset.Now);

            try
            {
                string stagingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StagingFiles");
                Directory.CreateDirectory(stagingPath);

                _logger.LogInformation("Extrayendo vistas desde la Base de Datos relacional...");
                var customersDb = _databaseExtractor.ExtractDataFromView("View_Customers");
                var productsDb = _databaseExtractor.ExtractDataFromView("View_Products");

                if (customersDb.Rows.Count > 0)
                {
                    string path = Path.Combine(stagingPath, "Staging_DB_Customers.json");

                    var rowsList = customersDb.AsEnumerable().Select(row =>
                        customersDb.Columns.Cast<DataColumn>().ToDictionary(
                            col => col.ColumnName,
                            col => row[col] == DBNull.Value ? null : row[col]
                        )
                    ).ToList();

                    var jsonString = JsonSerializer.Serialize(rowsList, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(path, jsonString, stoppingToken);
                    _logger.LogInformation("Clientes de BD guardados en temporal: {path}", path);
                }

                _logger.LogInformation("Leyendo datos desde archivos CSV...");
                string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArchivosOrigen", "clientes.csv");

                if (File.Exists(csvFilePath))
                {
                    var csvCustomers = _csvExtractor.ExtractCustomers(csvFilePath);
                    string path = Path.Combine(stagingPath, "Staging_CSV_Customers.json");
                    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(csvCustomers), stoppingToken);
                    _logger.LogInformation("Datos de CSV guardados en temporal: {path}", path);
                }
                else
                {
                    _logger.LogWarning("No se encontró el archivo CSV en la ruta especificada para la extracción.");
                }

                _logger.LogInformation("Consumiendo datos desde la API REST...");
                try
                {
                    var apiProducts = await _apiExtractor.ExtractProductsFromApiAsync("productos-endpoint");
                    if (apiProducts != null)
                    {
                        string path = Path.Combine(stagingPath, "Staging_API_Products.json");
                        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(apiProducts), stoppingToken);
                        _logger.LogInformation("Productos de API guardados en temporal: {path}", path);
                    }
                }
                catch (Exception apiEx)
                {
                    _logger.LogWarning(apiEx, "No se pudo conectar a la API REST, continuando con el flujo de extracción local.");
                }

                stopwatch.Stop();
                _logger.LogInformation("Proceso de extracción completado exitosamente en {elapsedTime} ms.", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error crítico durante el proceso de extracción.");
            }
        }
    }
}
