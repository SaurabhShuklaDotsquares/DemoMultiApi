using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Taxs
{
    public interface ITaxService:IDisposable
    {
        PagedListResult<Tax> GetTaxByPage(SearchQuery<Tax> query, out int totalItems);
        Tax GetById(int id);
        List<Tax> GetTaxList();
        Tax UpdateTax(Tax ReturnItems);
        Tax GetByTax(byte ReturnItemsType);
        Tax SaveTax(Tax ReturnItems);
        string GetNameById(int? id);
        bool Delete(int id);
        Tax GetTaxByCategoryId(int categoryId);
        List<Tax> checkCategoryExist(int categoryId);
    }
}
