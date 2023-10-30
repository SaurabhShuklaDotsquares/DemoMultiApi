using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IBrandsService :IDisposable
    {
        PagedListResult<Brands> Get(SearchQuery<Brands> query, out int totalItems);
        Brands GetById(int id);
        Brands Save(Brands entity);
        Brands Update(Brands entity);
        bool Delete(int id);
        List<Brands> GetBrandsList();
        List<Brands> GetBrandsList(DateTime? S_date, DateTime? E_date);
        Brands GetByTitle(string title);
    }
}
