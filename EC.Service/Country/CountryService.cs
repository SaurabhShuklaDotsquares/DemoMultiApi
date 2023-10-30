using EC.Data.Models;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class CountryService : ICountryService 
    {
        private readonly IRepository<Countries> _repoCountry;

        public CountryService(IRepository<Countries> repoCountry)
        {
            _repoCountry = repoCountry;
        }
        public Countries GetBycode(string Sortname)
        {
            return _repoCountry.Query().Filter(x => x.Sortname == Sortname).AsNoTracking().Get().FirstOrDefault();
        }
        public Countries GetCountries(string Sortname)
        {
            return _repoCountry.Query().Filter(x => x.Sortname == Sortname).Get().FirstOrDefault();
        }
        public Countries GetById(int id)
        {
            return _repoCountry.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public IEnumerable<Countries> GetCountries()
        {
            return _repoCountry.Query().AsNoTracking().Get().ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            _repoCountry?.Dispose();
        }
        #endregion
    }
}
