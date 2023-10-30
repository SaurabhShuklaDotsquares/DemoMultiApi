using EC.Data.Models;
using EC.DataTable.Search;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.AllPages
{
    public interface IPagesService:IDisposable
    {
        PagedListResult<Pages> GetPagesByPage(SearchQuery<Pages> query, out int totalItems);
        Pages GetById(int id);
        List<Pages> Getlist();
        Pages UpdatePages(Pages Pages);
        Pages SavePages(Pages Pages);
        bool Delete(int id);
        Pages GetByTitle(string title);
    }
}
