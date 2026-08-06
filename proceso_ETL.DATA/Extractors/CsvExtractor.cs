using System.Globalization;
using CsvHelper;
using proceso_ETL.DATA.Interfaces;
using proceso_ETL.DATA.Models;

namespace proceso_ETL.DATA.Extractors
{
    public class CsvExtractor : ICsvExtractor
    {
        public IEnumerable<CustomerDto> ExtractCustomers(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo CSV de clientes no fue encontrado en la ruta: {filePath}");
            }

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<CustomerDto>().ToList();
        }

        public IEnumerable<ProductDto> ExtractProducts(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo CSV de productos no fue encontrado en la ruta: {filePath}");
            }

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<ProductDto>().ToList();
        }

        public IEnumerable<OrderDto> ExtractOrders(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo CSV de órdenes no fue encontrado en la ruta: {filePath}");
            }

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<OrderDto>().ToList();
        }

        public IEnumerable<OrderDetailDto> ExtractOrderDetails(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo CSV de detalles de órdenes no fue encontrado en la ruta: {filePath}");
            }

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<OrderDetailDto>().ToList();
        }
    }
}
