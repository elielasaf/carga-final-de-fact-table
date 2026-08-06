using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proceso_ETL.DATA.Loaders
{
    public class DataLoader
    {
        private readonly string _connectionString;

        public DataLoader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task LoadDataToStagingOrWarehouseAsync(DataTable dataTable, string targetTableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = targetTableName;

                    try
                    {
                        await bulkCopy.WriteToServerAsync(dataTable);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error al cargar datos masivos en la tabla {targetTableName}: {ex.Message}");
                    }
                }
            }
        }
    }
}
