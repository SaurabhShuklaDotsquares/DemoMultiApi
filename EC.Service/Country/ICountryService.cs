using EC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ICountryService:IDisposable
    {
        IEnumerable<Countries> GetCountries();
        Countries GetCountries(string Sortname);
        Countries GetBycode(string sortname);
        Countries GetById(int id);

    }
}
