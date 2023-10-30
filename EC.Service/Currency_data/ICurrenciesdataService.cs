using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Currency_data
{
    public interface ICurrenciesdataService:IDisposable
    {
        PagedListResult<CurrencyData> GetCurrencyDataByPage(SearchQuery<CurrencyData> query, out int totalItems);
        CurrencyData GetById(int id);
        List<CurrencyData> GetCurrencyDataList();
        CurrencyData UpdateCurrencyData(CurrencyData Currencie);
        CurrencyData GetByCurrencyData(byte CurrencieType);
        CurrencyData SaveCurrencyData(CurrencyData Currencie);
        bool Delete(long id);
    }
}
