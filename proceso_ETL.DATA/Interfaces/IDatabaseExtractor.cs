using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace proceso_ETL.DATA.Interfaces
{
    public interface IDatabaseExtractor
    {
        DataTable ExtractDataFromView(string viewName);
    }
}
