using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EC.Service
{
    public class BrandsService :IBrandsService
    {
        /// <summary>
        /// The repo Brands
        /// </summary>
        IRepository<Brands> _repoBrands;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrandsService"/> class.
        /// </summary>
        /// <param name="_repoBrands">The repo Brands.</param>
        public BrandsService(IRepository<Brands> repoBrands)
        {
            _repoBrands = repoBrands;
        }
        public PagedListResult<Brands> Get(SearchQuery<Brands> query, out int totalItems)
        {
            return _repoBrands.Search(query, out totalItems);
        }
        public Brands GetById(int id)
        {
            return _repoBrands.FindById(id);
        }
        public Brands GetByTitle(string title)
        {
            return _repoBrands.Query().Filter(x => x.Title == title).Get().FirstOrDefault();
        }
        public Brands Save(Brands entity)
        {
            _repoBrands.Insert(entity);
            return entity;
        }
        public Brands Update(Brands entity)
        {
            _repoBrands.Update(entity);
            return entity;
        }
        public bool Delete(int id)
        {
            Brands brands = _repoBrands.FindById(id);
            _repoBrands.Delete(brands);
            return true;
        }
        public List<Brands> GetBrandsList()
        {
            return _repoBrands.Query().Filter(x=> x.Status == true).Get().ToList();
        }
        public List<Brands> GetBrandsList(DateTime? S_date, DateTime? E_date)
        {
            return _repoBrands.Query().Get().Where(x => x.CreatedAt >= S_date && x.UpdatedAt.Value.Date <= E_date).ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (_repoBrands != null)
            {
                _repoBrands.Dispose();
            }
        }
        #endregion
    }
}
