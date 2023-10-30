using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class TemplateEmailService : ITemplateEmailService
    {
        private readonly IRepository<Emails> _repoEmail;

        public TemplateEmailService(IRepository<Emails> repoEmail)
        {
            _repoEmail = repoEmail;
        }
        public PagedListResult<Emails> GetEmailsByPage(SearchQuery<Emails> query, out int totalItems)
        {
            return _repoEmail.Search(query, out totalItems);
        }
        public Emails GetById(int id)
        {
            return _repoEmail.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public Emails GetByName(string Name)
        {
            return _repoEmail.Query().Filter(x => x.Name == Name).Get().FirstOrDefault();
        }
        public Emails GetByEmailType(byte emailType)
        {
            return _repoEmail.Query().Get().FirstOrDefault();
        }
        public List<Emails> GetEmailsList()
        {
            var data= _repoEmail.Query().Get().ToList();
            return data;
        }
        public Emails UpdateEmails(Emails Pages)
        {
            _repoEmail.Update(Pages);
            return Pages;
        }
        public Emails SaveEmails(Emails Pages)
        {
            _repoEmail.Insert(Pages);
            return Pages;
        }
        public bool Delete(int id)
        {
            Emails pages = _repoEmail.FindById(id);
            _repoEmail.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            if (_repoEmail!=null)
            {
                 _repoEmail.Dispose();
            }
        }
    }
}
