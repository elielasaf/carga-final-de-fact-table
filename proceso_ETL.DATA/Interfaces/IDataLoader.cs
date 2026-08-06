using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace proceso_ETL.DATA.Interfaces
{
    public interface IDataLoader
    {
        Task LoadDataToStagingOrWarehouseAsync(System.Data.DataTable dataTable, string targetTableName);
        Task ExecuteStoredProcedureAsync(string storedProcedureName, SqlParameter[]? parameters = null);
    }
}
