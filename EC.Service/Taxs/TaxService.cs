using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Taxs
{
    public class TaxService : ITaxService
    {
        private readonly IRepository<Tax> _repoTax;
        public TaxService(IRepository<Tax> repoTax)
        {
            _repoTax = repoTax;
        }
        public string GetNameById(int? id)
        {
            return _repoTax.Query().Filter(x => x.Id == id).Get().Select(x => x.Title).FirstOrDefault();
        }
        public Tax GetById(int id)
        {
            return _repoTax.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public Tax GetByTax(byte TaxType)
        {
            return _repoTax.Query().Get().FirstOrDefault();
        }
        public PagedListResult<Tax> GetTaxByPage(SearchQuery<Tax> query, out int totalItems)
        {
            return _repoTax.Search(query, out totalItems);
        }
        public List<Tax> GetTaxList()
        {
            return _repoTax.Query().Get().ToList();
        }
        public Tax SaveTax(Tax tax)
        {
            _repoTax.Insert(tax);
            return tax;
        }
        public Tax UpdateTax(Tax tax)
        {
            _repoTax.Update(tax);
            return tax;
        }
        public bool Delete(int id)
        {
            Tax pages = _repoTax.FindById(Convert.ToInt64(id));
            _repoTax.Delete(pages);
            return true;
        }
        public Tax GetTaxByCategoryId(int categoryId)
        {
            return _repoTax.Query().Filter(x => x.CategoryId == categoryId).Get().FirstOrDefault();
        }
        public List<Tax> checkCategoryExist(int categoryId)
        {
            return _repoTax.Query().Filter(x => x.CategoryId == categoryId).Get().ToList();
        }
        public void Dispose()
        {
            if (_repoTax != null)
            {
                _repoTax.Dispose();
            }
        }
    }
}
