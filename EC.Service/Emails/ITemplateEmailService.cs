using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ITemplateEmailService:IDisposable
    {
        PagedListResult<Emails> GetEmailsByPage(SearchQuery<Emails> query, out int totalItems);
        Emails GetById(int id);
        List<Emails> GetEmailsList();
        Emails UpdateEmails(Emails Pages);
        Emails GetByEmailType(byte emailType);
        Emails SaveEmails(Emails Pages);
        bool Delete(int id);
       Emails GetByName(string Name);
    }
}
