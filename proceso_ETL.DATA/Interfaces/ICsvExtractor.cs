using proceso_ETL.DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace proceso_ETL.DATA.Interfaces
{
    public interface ICsvExtractor
    {
        IEnumerable<CustomerDto> ExtractCustomers(string filePath);
        IEnumerable<ProductDto> ExtractProducts(string filePath);
        IEnumerable<OrderDto> ExtractOrders(string filePath);
        IEnumerable<OrderDetailDto> ExtractOrderDetails(string filePath);
    }
}
