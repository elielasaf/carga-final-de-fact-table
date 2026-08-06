using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using proceso_ETL.DATA.Interfaces;

namespace proceso_ETL.DATA.Loaders
{
    public class DataLoader : IDataLoader
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

        public async Task ExecuteStoredProcedureAsync(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error al ejecutar el procedimiento almacenado {storedProcedureName}: {ex.Message}");
                    }
                }
            }
        }
    }
}
