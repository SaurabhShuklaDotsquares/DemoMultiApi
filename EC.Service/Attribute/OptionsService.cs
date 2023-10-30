using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class OptionsService : IOptionsService
    {
        /// <summary>
        /// The repo Attributes
        /// </summary>
        IRepository<Options> _repoAttribute;
        IRepository<OptionValues> _repoOptionValues;
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsService"/> class.
        /// </summary>
        /// <param name="_repoAttribute">The repo Attribute.</param>
        public OptionsService(IRepository<Options> repoAttribute, IRepository<OptionValues> repoOptionValues)
        {
            _repoAttribute = repoAttribute;
            _repoOptionValues = repoOptionValues;
        }
        public PagedListResult<Options> Get(SearchQuery<Options> query, out int totalItems)
        {
            return _repoAttribute.Search(query, out totalItems);
        }
        public Options GetById(int id)
        {
            return _repoAttribute.FindById(id);
        }
        public Options GetOptionsById(int id)
        {
            return _repoAttribute.Query().Filter(x => x.Id == id).Include(x=>x.OptionValues).Get().FirstOrDefault();
        }
        public Options GetByTitle(string title)
        {
            return _repoAttribute.Query().Filter(x => x.Title == title).Get().FirstOrDefault();
        }
        public Options Save(Options entity)
        {
            _repoAttribute.Insert(entity);
            return entity;
        }
        public Options Update(Options entity)
        {
            _repoAttribute.Update(entity);
            return entity;
        }
        public bool Delete(int id)
        {
                Options options = _repoAttribute.FindById(id);
                 _repoAttribute.Delete(options);
                return true;
        }
        public List<Options> GetOptionsList()
        {
            return _repoAttribute.Query().Filter(x=> x.Status == true).Include(x=> x.OptionValues).Get().ToList();
        }
        public Options GetOptionsWithOptionValueById(int id, int[] attributeValue)
        {
            List<OptionValues> lstResult = new List<OptionValues>();
            lstResult = _repoOptionValues.Query().Filter(m => m.OptionId == id).Include(x => x.Option).Get().ToList();
            if (attributeValue.Any() && lstResult.Any())
            {
                lstResult = lstResult.Where(x => attributeValue.Contains(x.Id))?.ToList() ?? new List<OptionValues>();
            }

            Options option = lstResult != null && lstResult.Any() ? lstResult.FirstOrDefault().Option : new Options();
            option.OptionValues = lstResult;
            return option;
        }
        #region "Dispose"
        public void Dispose()
        {
            if (_repoAttribute != null)
            {
                _repoAttribute.Dispose();
            }
        }
        #endregion
    }
}
