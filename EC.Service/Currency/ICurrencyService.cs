using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.Currency
{
    public interface ICurrencyService:IDisposable
    {
        PagedListResult<Currencies> GetCurrenciesByPage(SearchQuery<Currencies> query, out int totalItems);
        Currencies GetById(int id);
        List<Currencies> GetCurrenciessList();
        Currencies UpdateCurrencies(Currencies Currencie);
        Currencies GetByCurrencies(byte CurrencieType);
        Currencies SaveCurrencies(Currencies Currencie);
        bool Delete(int id);
    }
}
