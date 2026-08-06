using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using proceso_ETL.DATA.Models;

namespace proceso_ETL.DATA.Interfaces
{
    public interface IApiExtractor
    {
        Task<IEnumerable<ProductDto>> ExtractProductsFromApiAsync(string endpoint);
    }
}
