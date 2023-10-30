using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    
    public class ContactUsService: IContactUsService
    {
        private readonly IRepository<ContuctUs> _repoContactUs;
        public ContactUsService(IRepository<ContuctUs> repoContactUs)
        {
            _repoContactUs=repoContactUs; 
        }
        public PagedListResult<ContuctUs> Get(SearchQuery<ContuctUs> query, out int totalItems)
        {
            return _repoContactUs.Search(query, out totalItems);
        }
        public ContuctUs GetById(int id)
        {
            return _repoContactUs.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public ContuctUs GetByVendoremail(string email)
        {
            return _repoContactUs.Query().Filter(x => x.Email == email).Get().FirstOrDefault();
        }
        public ContuctUs GetByVendorproductId(int id)
        {
            return _repoContactUs.Query().Filter(x => x.ProductId == id).Get().FirstOrDefault();
        }
        public List<ContuctUs> GetVendorContactList(DateTime? S_date, DateTime? E_date,int id)
        {
            var dd = _repoContactUs.Query().Filter(x=>x.ProductId==id).Get().Where(x => x.CreatedAt.Value.Date >= S_date && x.UpdatedAt.Value.Date <= E_date).ToList();
            return dd;
        }

        public ContuctUs Save(ContuctUs Contuct)
        {
            _repoContactUs.Insert(Contuct);
            return Contuct;
        }
        public void Dispose()
        {
            if (_repoContactUs != null)
            {
                _repoContactUs.Dispose();
            }
        }
    }
}
