using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IOptionsService:IDisposable
    {
        PagedListResult<Options> Get(SearchQuery<Options> query, out int totalItems);
        Options GetById(int id);
        Options GetOptionsById(int id);
        Options Save(Options entity);
        Options Update(Options entity);
        bool Delete(int id);
        List<Options> GetOptionsList();
        Options GetByTitle(string title);
        Options GetOptionsWithOptionValueById(int id, int[] attributeValue);
    }
}
