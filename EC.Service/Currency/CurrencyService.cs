using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Currency
{
    public class CurrencyService:ICurrencyService
    {
        private readonly IRepository<Currencies> _repoCurrencie;
        public CurrencyService(IRepository<Currencies> repoCurrencie)
        {
            _repoCurrencie = repoCurrencie;
        }
        public PagedListResult<Currencies> GetCurrenciesByPage(SearchQuery<Currencies> query, out int totalItems)
        {
            return _repoCurrencie.Search(query, out totalItems);
        }
        public Currencies GetById(int id)
        {
            return _repoCurrencie.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public List<Currencies> GetCurrenciessList()
        {
            return _repoCurrencie.Query().Include(x=>x.CurrencyData).Get().ToList();
        }
        public Currencies UpdateCurrencies(Currencies Currencie)
        {
            _repoCurrencie.Update(Currencie);
            return Currencie;
        }
        public Currencies GetByCurrencies(byte CurrencieType)
        {
            return _repoCurrencie.Query().Get().FirstOrDefault();
        }
        public Currencies SaveCurrencies(Currencies Currencie)
        {
            _repoCurrencie.Insert(Currencie);
            return Currencie;
        }
        public bool Delete(int id)
        {
            Currencies pages = _repoCurrencie.FindById(id);
            _repoCurrencie.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            if (_repoCurrencie != null)
            {
                _repoCurrencie.Dispose();
            }
        }
    }
}
