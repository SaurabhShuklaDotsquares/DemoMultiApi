using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service.Currency_data
{
    public class CurrenciesdataService : ICurrenciesdataService
    {
        private readonly IRepository<CurrencyData> _repoCurrencyData;
        public CurrenciesdataService(IRepository<CurrencyData> repoCurrencyData)
        {
            _repoCurrencyData = repoCurrencyData;
        }
        public bool Delete(long id)
        {
            CurrencyData pages = _repoCurrencyData.FindById(id);
            _repoCurrencyData.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            if (_repoCurrencyData != null)
            {
                _repoCurrencyData.Dispose();
            }
        }
        public CurrencyData GetByCurrencyData(byte CurrenciedataType)
        {
            return _repoCurrencyData.Query().Get().FirstOrDefault();
        }
        public CurrencyData GetById(int id)
        {
            return _repoCurrencyData.Query().Filter(x => x.Id == id).Include(x=>x.Currency).Get().FirstOrDefault();
        }
        public PagedListResult<CurrencyData> GetCurrencyDataByPage(SearchQuery<CurrencyData> query, out int totalItems)
        {
            return _repoCurrencyData.Search(query, out totalItems);
        }
        public List<CurrencyData> GetCurrencyDataList()
        {
            // return _repoCurrencyData.Query().Get().ToList();
            return _repoCurrencyData.Query()
                 .Filter(x => x.Status == true)
                 .Include(x => x.Currency)
                 .Get()
                 .GroupBy(x => x.CurrencyId)
                 .Select(x => x.FirstOrDefault()).ToList();
        }
        public CurrencyData SaveCurrencyData(CurrencyData Currenciedata)
        {
            _repoCurrencyData.Insert(Currenciedata);
            return Currenciedata;
        }
        public CurrencyData UpdateCurrencyData(CurrencyData Currenciedata)
        {
            _repoCurrencyData.Update(Currenciedata);
            return Currenciedata;
        }

        
    }
}
