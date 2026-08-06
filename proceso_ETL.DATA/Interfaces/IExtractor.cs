using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace proceso_ETL.DATA.Interfaces
{
    public interface IExtractor
    {
        Task ExtractAsync();
    }
}
