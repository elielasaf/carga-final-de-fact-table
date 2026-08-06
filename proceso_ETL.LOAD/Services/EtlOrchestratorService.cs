using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using proceso_ETL.DATA.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proceso_ETL.LOAD.Services
{
    public class EtlOrchestratorService
    {
        private readonly ILogger<EtlOrchestratorService> _logger;
        private readonly IDataLoader _dataLoader;

        public EtlOrchestratorService(ILogger<EtlOrchestratorService> logger, IDataLoader dataLoader)
        {
            _logger = logger;
            _dataLoader = dataLoader;
        }

        public async Task RunLoadProcessAsync()
        {
            _logger.LogInformation("Iniciando fase de Carga (L de ETL) hacia el Data Warehouse desde el servicio de Load...");

            _logger.LogInformation("Poblando DimGeography...");
            await _dataLoader.ExecuteStoredProcedureAsync("sp_LoadDimGeography");

            _logger.LogInformation("Poblando DimProduct...");
            await _dataLoader.ExecuteStoredProcedureAsync("sp_LoadDimProduct");

            _logger.LogInformation("Poblando DimCustomer...");
            await _dataLoader.ExecuteStoredProcedureAsync("sp_LoadDimCustomer");

            _logger.LogInformation("Poblando DimDate...");
            var startDateParam = new SqlParameter("@StartDate", SqlDbType.Date) { Value = "2023-01-01" };
            var endDateParam = new SqlParameter("@EndDate", SqlDbType.Date) { Value = "2025-12-31" };
            await _dataLoader.ExecuteStoredProcedureAsync("sp_LoadDimDate", new[] { startDateParam, endDateParam });

            _logger.LogInformation("Poblando FactSales...");
            await _dataLoader.ExecuteStoredProcedureAsync("sp_LoadFactSales");

            _logger.LogInformation("Carga del Data Warehouse finalizada con éxito.");
        }
    }
}
