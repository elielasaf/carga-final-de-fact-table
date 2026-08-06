using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using proceso_ETL.DATA.Models;

namespace proceso_ETL.DATA.Extractors
{
    public class DatabaseExtractor
    {
        private readonly string _connectionString;

        public DatabaseExtractor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExtractDataFromView(string viewName)
        {
            DataTable dataTable = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = $"SELECT * FROM [dbo].[{viewName}]";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public DataTable ExtractCustomers() => ExtractDataFromView("View_Customers");

        public DataTable ExtractProducts() => ExtractDataFromView("View_Products");

        public DataTable ExtractOrdersDetails() => ExtractDataFromView("View_OrdersDetails");
    }
}
