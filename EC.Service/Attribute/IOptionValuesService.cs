using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IOptionValuesService : IDisposable
    {
        List<OptionValues> GetOptionValuesList();
        OptionValues Save(OptionValues entity);
        OptionValues Update(OptionValues entity);
        bool DeleteByOptionValueId(int id);
        int Delete(int id);
        List<OptionValues> GetOptionValuesById(int? id);
    }
}
