using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IContactUsService:IDisposable
    {
        public PagedListResult<ContuctUs> Get(SearchQuery<ContuctUs> query, out int totalItems);
        ContuctUs GetById(int id);
        ContuctUs GetByVendoremail(string email);
        ContuctUs GetByVendorproductId(int id);
        ContuctUs Save(ContuctUs Contuct);
        List<ContuctUs> GetVendorContactList(DateTime? S_date, DateTime? E_date, int id);
    }
}
